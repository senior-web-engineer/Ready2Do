/*
Crea un nuovo appuntamento per l'utente.

Parametri:
 @pIdCliente: identificativo del cliente presso cui si sta fissando l'appuntamento
 @pUserId: Utente per cui l'appuntamento viene creato, se null indica un appuntamento per un utente GUEST
 @pIdAbboanmento: Se specificato, altrimenti si  prende il primo in ordine cronologico in caso ce ne sia più di uno applicabile,
				  indica su quale abbonamento usare per

RETURNS:
	 1: OK
	-1: Nessun abbonamento valido per l'utente - evento
	-4: Data Chisura Iscrizione superata
    -5: Nessun posto disponibile per l'evento
NOTE: 
Le logiche di gestione degli abbonamenti dovranno essere riviste per gestire le "compatibilità" (livelli?) tra lezioni ed abbonamenti.
*/
CREATE PROCEDURE [dbo].[Appuntamenti_Add]
	@pIdCliente			INT,
	@pUserId			VARCHAR(50),
	@pScheduleId		INT,
	@pIdAbbonamento		INT = NULL,
	@pNote				NVARCHAR(1000),
	@pNominativo		NVARCHAR(200),
	@pIdAppuntamento	INT OUTPUT
AS
BEGIN
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
SET XACT_ABORT ON;

DECLARE @dtOperazione		DATETIME2 = SYSDATETIME(),
		@idAbbonamento		INT = NULL,
		@numIngressiResidui INT = NULL,
		@numPostiResidui	INT = NULL,
		@dataChiusuraIscriz	DATETIME2 = NULL

-- Verifichiamo che la data termine per le iscrizioni non sia stata superata
SELECT @numPostiResidui = s.PostiResidui,
	   @dataChiusuraIscriz = s.DataChiusuraIscrizioni
FROM Schedules s
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
	IF @pUserId IS NOT NULL
	BEGIN
		-- Acquisiamo il lock in update così che non possa essere aggiornato concorrentemente per tutta la durata della transazione
		SELECT TOP 1 
			@idAbbonamento = Id ,
			@numIngressiResidui = au.IngressiResidui
		FROM AbbonamentiUtenti au WITH (UPDLOCK)
		WHERE au.IdCliente = @pIdCliente 
		AND au.UserId = @pUserId
		AND au.DataCancellazione IS NULL
		AND au.Scadenza < @dtOperazione
		AND ((au.IngressiResidui IS NULL) OR (au.IngressiResidui > 0))
		AND ((@pIdAbbonamento IS NULL) OR (au.Id = @pIdAbbonamento))
		ORDER BY au.DataCreazione

		-- Verifichiamo che sia stato trovato un abbonamento valido
		IF @idAbbonamento IS NULL
		BEGIN
			RAISERROR(N'Impossibile trovare un abboanamento su cui addebitare l''appuntamento', 16, 0);
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

COMMIT;
RETURN 1

END