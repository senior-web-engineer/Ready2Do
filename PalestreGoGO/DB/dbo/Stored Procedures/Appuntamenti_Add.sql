/*
Crea un nuovo appuntamento per l'utente.

Parametri:
 @pIdCliente: identificativo del cliente presso cui si sta fissando l'appuntamento
 @pUserId: Utente per cui l'appuntamento viene creato, se null indica un appuntamento per un utente GUEST
 @pIdAbbonamento: Se NON specificato sta ad indicare un AppuntamentoDaConfermare, facciamo però il controllo che effettivamente non esista un abbonamento valido per l'utente

RETURNS:
	 1: OK
	-1: Nessun abbonamento valido per l'utente - evento
	-4: Data Chisura Iscrizione superata
    -5: Nessun posto disponibile per l'evento
	-8: IdAbbonamento non specificato ma abbonamento esistente per l'utente
ATTENZIONE:
La procedura ritorna un primo recordset (scalare) per indicare il tipo di appuntamento registrato
Il secondo recordset contiene il dettaglio dello specifico tipo (Appuntamento o AppuntamentoDaConfermare)

NOTE: 

*/
CREATE PROCEDURE [dbo].[Appuntamenti_Add]
	@pIdCliente					INT,
	@pUserId					VARCHAR(50),
	@pScheduleId				INT,
	@pIdAbbonamento				INT = NULL,
	@pNote						NVARCHAR(1000),
	@pNominativo				NVARCHAR(200),
	@pTimeoutManagerPayload		NVARCHAR(MAX),
	@pIdAppuntamento			INT OUTPUT
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
SET XACT_ABORT ON;

DECLARE @dtOperazione		DATETIME2 = SYSDATETIME(),
		@idAbbonamento		INT = NULL,
		@numIngressiResidui INT = NULL,
		@numPostiResidui	INT = NULL,
		@livelloLezione		INT = NULL,
		@dataChiusuraIscriz	DATETIME2 = NULL,
		@expirationWindowMinutes	INT = NULL;

-- Verifichiamo che la data termine per le iscrizioni non sia stata superata
SELECT @numPostiResidui = s.PostiResidui,
	   @dataChiusuraIscriz = COALESCE(s.DataChiusuraIscrizioni, s.[DataOraInizio]),
	   @livelloLezione = tl.Livello
FROM Schedules s
	INNER JOIN TipologieLezioni tl ON s.IdTipoLezione = tl.Id
WHERE s.Id = @pScheduleId

IF COALESCE(@dataChiusuraIscriz, '1900-01-01') < @dtOperazione
BEGIN
	RAISERROR(N'Il termine per l''iscrizione è scaduto.', 16, 0);
	RETURN -4;
END
IF COALESCE(@numPostiResidui, -1) <= 0
BEGIN
	RAISERROR(N'Nessun posto disponibile per l''evento',16, 0);
	RETURN -5
END

BEGIN TRANSACTION
	-- APPUNTAMENTI PER UTENTE NOTO
	IF @pUserId IS NOT NULL
	BEGIN
		-- Appuntamento da confermare
		IF @pIdAbbonamento IS NULL
		BEGIN
			-- Se non specificato un IdAbbonamento, l'utente non deve avere un abbonamento valido
			IF EXISTS(SELECT * FROM AbbonamentiUtenti au INNER JOIN TipologieAbbonamenti ta ON au.IdTipoAbbonamento = ta.Id
								WHERE au.IdCliente = @pIdCliente AND au.UserId = @pUserId AND au.DataCancellazione IS NULL 
											AND au.Scadenza > @dtOperazione 
											AND ((ta.MaxLivCorsi IS NULL) OR (ta.MaxLivCorsi >= @livelloLezione)))
			BEGIN
				RAISERROR(N'IdAbbonamento non specificato ma l''utente ha almeno un abbonamento valido',16, 0);
				RETURN -8
			END
			--Recuperiamo la finestra di timeout di un AppuntamentoDaConfermare per il cliente. Se non configurata usiamo un default di 48h
			SELECT @expirationWindowMinutes = COALESCE([Value], (48*60)) FROM ClientiPreferenze WHERE [Key] = 'APPUNTAMENTIDACONFERMARE.EXPIRATION.WINDOW.MINUTES'
			-- Andiamo a calcore la finestra effettiva tenendo conto della data chiusura iscrizioni
			SELECT @expirationWindowMinutes  = CASE WHEN DATEDIFF(minute, @dtOperazione, @dataChiusuraIscriz) > @expirationWindowMinutes THEN @expirationWindowMinutes
												ELSE DATEDIFF(minute, @dtOperazione, @dataChiusuraIscriz) 
												END
			-- Inseriamo un AppuntamentoDaConfermare 
			INSERT INTO AppuntamentiDaConfermare(IdCliente, UserId, ScheduleId, DataCreazione, DataExpiration, TimeoutManagerPayload)
				VALUES(@pIdCliente, @pUserId, @pScheduleId, @dtOperazione, DATEADD(minute,@expirationWindowMinutes,@dtOperazione), @pTimeoutManagerPayload)

			SET @pIdAppuntamento = SCOPE_IDENTITY();
			-- Ritorno il tipo di appuntamento
			SELECT 'DaConfermare' AS TipoAppuntamento
			-- e come secondo recordset il dettaglio dell'appuntamento da confermare
			SELECT * FROM AppuntamentiDaConfermare AS adc WHERE Id = @pIdAppuntamento
		END
		ELSE
		-- Appuntamento in caso di IdAbbonamento specificato
		BEGIN
			-- Verifico che l'abbonamento passato sia coerente con gli altri parametri e valido e compatibile con l'evento
			-- Acquisiamo il lock in update così che non possa essere aggiornato concorrentemente per tutta la durata della transazione
			SELECT TOP 1 
				@idAbbonamento = au.Id,
				@numIngressiResidui = au.IngressiResidui
			FROM AbbonamentiUtenti au WITH (UPDLOCK)
				INNER JOIN TipologieAbbonamenti ta  ON ta.Id = au.IdTipoAbbonamento
			WHERE au.IdCliente = @pIdCliente 
			AND au.UserId = @pUserId
			AND au.Id = @pIdAbbonamento
			AND au.DataCancellazione IS NULL
			AND au.Scadenza > @dtOperazione
			AND ((au.IngressiResidui IS NULL) OR (au.IngressiResidui > 0))
			AND ((ta.MaxLivCorsi IS NULL) OR (ta.MaxLivCorsi >= @livelloLezione))
			ORDER BY au.DataCreazione

			-- Verifichiamo che sia stato trovato un abbonamento valido
			IF @idAbbonamento IS NULL
			BEGIN
				RAISERROR(N'Impossibile trovare l''abboanamento su cui addebitare l''appuntamento', 16, 0);
				RETURN -1;
			END

			-- Scaliamo un ingresso dall'abbonamento (se previsti)
			IF COALESCE(@numIngressiResidui, -1) > 0
			BEGIN
				UPDATE AbbonamentiUtenti SET IngressiResidui = IngressiResidui -1 WHERE Id = @idAbbonamento;
				-- Loggiamo la transazione
				INSERT INTO AbbonamentiTransazioni (IdAbbonamento, Testo)
					SELECT @idAbbonamento,
						   CONCAT('Decrementato 1 Ingresso per Appuntamento a Schedule: ', @pScheduleId)
			END

			-- Inseriamo l'appuntamento
			INSERT INTO Appuntamenti (IdCliente, UserId, ScheduleId, IdAbbonamento, DataPrenotazione, Note, Nominativo)
				VALUES(@pIdCliente, @pUserId, @pScheduleId, @idAbbonamento, @dtOperazione, @pNote, @pNominativo)
			SET @pIdAppuntamento = SCOPE_IDENTITY();

			-- Riduciamo i posti disponibili per l'evento (schedule)
			UPDATE Schedules
				SET PostiResidui = PostiResidui -1
			WHERE Id = @pScheduleId
			AND PostiResidui > 0

			IF @@ROWCOUNT = 0
			BEGIN
				RAISERROR(N'Nessun posto disponibile per l''evento',16, 0);
				RETURN -5
			END

			-- Ritorno il tipo di appuntamento
			SELECT 'Confermato' AS TipoAppuntamento
			-- e come secondo recordset il dettaglio dell'appuntamento da confermare
			SELECT * FROM Appuntamenti WHERE Id = @pIdAppuntamento

		END 
	END
	-- APPUNTAMENTO PER UTENTE GUEST
	ELSE
	BEGIN
		-- Inseriamo l'appuntamento
		INSERT INTO Appuntamenti (IdCliente, UserId, ScheduleId, IdAbbonamento, DataPrenotazione, Note, Nominativo)
			VALUES(@pIdCliente, NULL, @pScheduleId, NULL, @dtOperazione, @pNote, @pNominativo)
		SET @pIdAppuntamento = SCOPE_IDENTITY();

		-- Riduciamo i posti disponibili per l'evento (schedule)
		UPDATE Schedules
			SET PostiResidui = PostiResidui -1
		WHERE Id = @pScheduleId
		AND PostiResidui > 0
		
		-- Ritorno il tipo di appuntamento
		SELECT 'Confermato' AS TipoAppuntamento
		-- e come secondo recordset il dettaglio dell'appuntamento da confermare
		SELECT * FROM Appuntamenti WHERE Id = @pIdAppuntamento
	END
COMMIT;
RETURN 1

END