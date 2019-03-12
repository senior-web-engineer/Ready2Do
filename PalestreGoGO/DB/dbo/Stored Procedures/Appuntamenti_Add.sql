/*
Crea un nuovo appuntamento per l'utente.

Parametri:
 @pIdCliente: identificativo del cliente presso cui si sta fissando l'appuntamento
 @pUserId: Utente per cui l'appuntamento viene creato, se null indica un appuntamento per un utente GUEST
 @pIdAbbonamento: Se NON specificato sta ad indicare un AppuntamentoDaConfermare, facciamo però il controllo che effettivamente non esista un abbonamento valido per l'utente

** GESTIONE WAITING LIST **
1.	Se per un Evento non ci sono posti disponibile ma è abilitata la Waiting List, la procedura, invece di creare un Appuntamento, genera un record nella Waiting List.
	Questo comportamento è possibile SOLO se specificato un IdAbbonamento, per gli AppuntamentiDaConfermare non gestiamo questa funzionalità (almeno per ora) perché
	la gestione si complicherebbe.
	D'altronde un A.D.C. non decremente i posti disponibili fino alla conferma dello stesso per cui non avrebbe molto senso metterlo in WL. Al momento della conferma eventualmente,
	se non ci sono posti disponibili potrebbe essere inserito in WL.
2.	Per gli utenti GUEST non ha senso (e non è tecnicamente possibile) gestire la WL

RETURNS:
	 1: OK
	-1: Nessun abbonamento valido per l'utente - evento
	-4: Data Chisura Iscrizione superata
    -5: Nessun posto disponibile per l'evento
	-8: IdAbbonamento non specificato ma abbonamento esistente per l'utente
   -10: Ingressi insufficienti per l'abbonamento utente

ATTENZIONE:
Ritorniamo un record del tipo:  TipoResult - JSON
TipoResult è una stringa che indica il tipo di contenuto JSON

~~~ CHANGES HISTORY ~~~
20190104
	- Gestione WaitingList
	- Cambiato formato ritorno, invece di tornare 2 record, ne ritorniamo uno solo con una colonna JSON con i dettagli
20190311
	- cambiato ritorno, non torniamo più il dettaglio dell'appuntamento (a prescindere dal tipo) ma solo l'Id e la tipologia
	  se interessati al dettaglio sarà necessario fare un'ulteriore lettura ma al momento non veniva utilizzato
*/
CREATE PROCEDURE [dbo].[Appuntamenti_Add]
	@pIdCliente					INT,
	@pUserId					VARCHAR(50),
	@pScheduleId				INT,
	@pIdAbbonamento				INT = NULL,
	@pNote						NVARCHAR(1000) = NULL,
	@pNominativo				NVARCHAR(200) = NULL,
	@pTimeoutManagerPayload		NVARCHAR(MAX) = NULL,
	@pIdAppuntamento			INT OUTPUT -- valorizzato solo viene preso effettivamente un abbonamento 
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
SET XACT_ABORT ON;

DECLARE @dtOperazione		DATETIME2 = SYSDATETIME(),
		@dtInizioSchedule	DATETIME2,
		@idAbbonamento		INT = NULL,
		@numIngressiResidui INT = NULL,
		@numPostiResidui	INT = NULL,
		@livelloLezione		INT = NULL,
		@idWL				INT = NULL,
		@waitListEnabled	BIT = 0,
		@dataChiusuraIscriz	DATETIME2 = NULL,
		@expirationWindowMinutes	INT = NULL;

-- Verifichiamo che la data termine per le iscrizioni non sia stata superata
SELECT @numPostiResidui = s.PostiResidui,
	   @dataChiusuraIscriz = COALESCE(s.DataChiusuraIscrizioni, s.[DataOraInizio]),
	   @livelloLezione = tl.Livello,
	   @waitListEnabled = COALESCE(WaitListDisponibile,0),
	   @dtInizioSchedule = DataOraInizio
FROM Schedules s
	INNER JOIN TipologieLezioni tl ON s.IdTipoLezione = tl.Id
WHERE s.Id = @pScheduleId

IF COALESCE(@dataChiusuraIscriz, '1900-01-01') < @dtOperazione
BEGIN
	RAISERROR(N'Il termine per l''iscrizione è scaduto.', 16, 0);
	RETURN -4;
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
				ROLLBACK
				RAISERROR(N'IdAbbonamento non specificato ma l''utente ha almeno un abbonamento valido',16, 0);
				RETURN -8
			END
			--Recuperiamo la finestra di timeout di un AppuntamentoDaConfermare per il cliente. Se non configurata usiamo un default di 48h
			SELECT @expirationWindowMinutes = COALESCE(CAST([Value] AS INT), (48*60)) FROM ClientiPreferenze WHERE [Key] = 'APPUNTAMENTIDACONFERMARE.EXPIRATION.WINDOW.MINUTES'
			-- Andiamo a calcore la finestra effettiva tenendo conto della data chiusura iscrizioni
			SELECT @expirationWindowMinutes  = CASE WHEN DATEDIFF(minute, @dtOperazione, @dataChiusuraIscriz) > @expirationWindowMinutes THEN @expirationWindowMinutes
												ELSE DATEDIFF(minute, @dtOperazione, @dataChiusuraIscriz) 
												END
			-- Inseriamo un AppuntamentoDaConfermare (NON TENIAMO CONTO DELLE DISPONIBILITA')
			INSERT INTO AppuntamentiDaConfermare(IdCliente, UserId, ScheduleId, DataCreazione, DataExpiration, TimeoutManagerPayload)
				VALUES(@pIdCliente, @pUserId, @pScheduleId, @dtOperazione, DATEADD(minute,@expirationWindowMinutes,@dtOperazione), @pTimeoutManagerPayload)

			SET @pIdAppuntamento = SCOPE_IDENTITY();
			-- Ritorno il tipo di appuntamento
			SELECT 'APPUNTAMENTO_DA_CONFERMARE' AS TipoAppuntamento, @pIdAppuntamento AS Id
					--[dbo].[internal_AppuntamentoDaConfermare_AsJSON](@pIdAppuntamento) AS [JSON]
			-- Terminiamo la transazione
			GOTO FINE_TRANS;
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
				ROLLBACK;
				RAISERROR(N'Impossibile trovare l''abboanamento su cui addebitare l''appuntamento', 16, 0);
				RETURN -1;
			END

			--Verifichiamo l'utente abbia ancora ingressi a disposizione (se NULL vuol dire che non sono gestiti gli ingressi)
			IF @numIngressiResidui = 0
			BEGIN
				ROLLBACK;
				RAISERROR(N'L''utente [%s] non dispone di ingressi residui sull''abbonamento specificato [IdAbbonamento: %i]', 16, 0, @pUserId, @pIdAbbonamento);
				RETURN -10;
			END
		
			-- Scaliamo un ingresso dall'abbonamento (se previsti)
			IF COALESCE(@numIngressiResidui, -1) > 0
			BEGIN
				UPDATE AbbonamentiUtenti SET IngressiResidui = IngressiResidui -1 WHERE Id = @idAbbonamento;
				EXEC [dbo].[internal_AbbonamentiUtenti_LogTransazione] @idAbbonamento, 'APP', -1, @dtOperazione, @pScheduleId, NULL
			END

			-- Decrementiamo i posti disponibili per l'evento (schedule)
			UPDATE Schedules
				SET PostiResidui = PostiResidui -1
			WHERE Id = @pScheduleId
			AND PostiResidui > 0

			--Se abbiamo aggiornato lo Schedules (quindi c'era un posto disponibile) ==> Generiamo l'appuntamento
			IF @@ROWCOUNT > 0
			BEGIN
				-- Inseriamo l'appuntamento
				INSERT INTO Appuntamenti (IdCliente, UserId, ScheduleId, IdAbbonamento, DataPrenotazione, Note, Nominativo)
					VALUES(@pIdCliente, @pUserId, @pScheduleId, @idAbbonamento, @dtOperazione, @pNote, @pNominativo)
				SET @pIdAppuntamento = SCOPE_IDENTITY();

				-- Ritorno il tipo di appuntamento
				SELECT 'APPUNTAMENTO_CONFERMATO' AS TipoAppuntamento, @pIdAppuntamento AS [Id]
					--[dbo].[internal_Appuntamento_AsJSON](@pIdAppuntamento) AS [JSON]

				GOTO FINE_TRANS;
			END
			-- Se invece non c'erano posti disponibili, inseriamo un record in Waiting List (se abilitata per lo schedule)
			ELSE
			BEGIN
				-- Se la WL non è abilitata ==> errore!
				IF COALESCE(@waitListEnabled,0) = 0
				BEGIN
					ROLLBACK;
					RAISERROR(N'Nessun posto disponibile per l''evento',16, 0);
					RETURN -5
				END
				ELSE
				BEGIN
					INSERT INTO ListeAttesa(IdCliente, IdSchedule, UserId, IdAbbonamento, DataScadenza)
						VALUES(@pIdCliente, @pScheduleId, @pUserId, @pIdAbbonamento, @dtInizioSchedule);
					
					SET @idWL = SCOPE_IDENTITY();
					EXEC [dbo].[internal_AbbonamentiUtenti_LogTransazione] @idAbbonamento, 'WLI', -1, @dtOperazione, NULL, @idWL

					SELECT 'WAITING_LIST' AS TipoAppuntamento, @idWL AS [Id]
						--[dbo].internal_ListaAttesa_AsJSON(@idWL) AS [JSON]
	
					GOTO FINE_TRANS;
				END
			END
		END 
	END
	-- APPUNTAMENTO PER UTENTE GUEST
	ELSE
	BEGIN
		--ATTENZIONE! Per gli utenti GUEST non gestiamo la lista d'attesa
		-- Inseriamo l'appuntamento (solo se ci sono posti disponibili)
		IF EXISTS(SELECT 1 FROM Schedules s WITH (UPDLOCK) WHERE Id = @pScheduleId AND PostiResidui > 0)
		BEGIN
			INSERT INTO Appuntamenti (IdCliente, UserId, ScheduleId, IdAbbonamento, DataPrenotazione, Note, Nominativo)
				VALUES(@pIdCliente, NULL, @pScheduleId, NULL, @dtOperazione, @pNote, @pNominativo)
			SET @pIdAppuntamento = SCOPE_IDENTITY();

			-- Riduciamo i posti disponibili per l'evento (schedule)
			UPDATE Schedules
				SET PostiResidui = PostiResidui -1
			WHERE Id = @pScheduleId
			AND PostiResidui > 0
			
			IF @@ROWCOUNT <> 1
			BEGIN
				ROLLBACK
				RAISERROR(N'Impossibile creare l''appuntamento GUEST poiché non ci sono disponibilità', 16, 0);
				RETURN -5
			END
			
			-- Ritorno il tipo di appuntamento
			SELECT 'APPUNTAMENTO_CONFERMATO' AS TipoAppuntamento, @pIdAppuntamento AS [Id]
			--[dbo].[internal_Appuntamento_AsJSON](@pIdAppuntamento) AS [JSON]
			GOTO FINE_TRANS;
		END
		ELSE
		BEGIN
			ROLLBACK;
			RAISERROR(N'Impossibile creare l''appuntamento GUEST poiché non ci sono disponibilità', 16, 0);
			RETURN -5
		END
	END
FINE_TRANS:
COMMIT;
RETURN 1
END