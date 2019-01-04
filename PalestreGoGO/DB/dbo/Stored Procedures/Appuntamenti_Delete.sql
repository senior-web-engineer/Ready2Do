/*
Annulla un appuntamento di un utente.
A seguito della cancellazione di un appuntamento si verificano le seguenti conseguenze:
- Se l'evento era FULL (PostiResidui = 0), la WaitingList per l'evento è abilitata e ci sono utenti in lista
	==> Viene creato un appuntamento per il primo utente della lista 

NOTA: Aggiungere l'utente tra i parametri?
*/
CREATE PROCEDURE [dbo].[Appuntamenti_Delete]
	@pIdCliente				INT,
	@pUserId				VARCHAR(100),
	@pIdAppuntamento		INT,
	@pEsitoOut				NVARCHAR(MAX) OUT --Json contenente l'esito della cancellazione
AS
BEGIN
	DECLARE @dtOperazione			DATETIME2 = SYSDATETIME(),
			@idAbbonamento			INT,
			@idSchedule				INT,
			@sogliaCancellazione	DATETIME2,
			@mustIncrementPosti		BIT = 0,
			@userIdWL				VARCHAR(100),
			@numRecordPromoted		INT

	DECLARE @tblUpdated TABLE(Id			INT NOT NULL, 
							  ScheduleId	INT NOT NULL, 
							  IdAbbonamento INT NULL, 
							  ScheduleCancellabileFinoAl DATETIME2  NULL, 
							  PostiResidui INT NOT NULL, 
							  WaitListAvailable BIT NOT NULL)
	DECLARE @tblUserWL TABLE(UserId VARCHAR(100) NOT NULL, IdAbbonamento INT NOT NULL, IdSchedule INT NOT NULL);

	BEGIN TRANSACTION		
	SET XACT_ABORT ON;
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

		-- Valorizziamo la DataCancellazione (SOLO se non già valorizzata)
		-- Andiamo in JOIN con lo Schedule per recuperare le informazioni di interesse
		-- e ci prendiamo un UPDLOCK (sul record specifico) così da evitare aggiornamenti concorrenti
		UPDATE A
			SET A.DataCancellazione = @dtOperazione
			OUTPUT inserted.Id, inserted.ScheduleId, inserted.IdAbbonamento, S.CancellabileFinoAl, S.CancellabileFinoAl, S.PostiResidui, COALESCE(S.WaitListDisponibile, 0)
				INTO @tblUpdated(Id, ScheduleId, IdAbbonamento, ScheduleCancellabileFinoAl, PostiResidui, WaitListAvailable)
		FROM Appuntamenti A
			INNER JOIN Schedules S WITH (UPDLOCK) ON A.ScheduleId = S.Id
		WHERE A.Id = @pIdAppuntamento
		AND A.IdCliente = @pIdCliente
		AND A.UserId = @pUserId
		AND A.DataCancellazione IS NULL
		-- La cancellazione è possibile solo se abilitata per l'evento
		AND COALESCE(S.CancellazioneConsentita, 0) = 1 

		IF @@ROWCOUNT = 0
		BEGIN
			RAISERROR(N'Parametri specificati non validi o prenotazione già annullata', 16, 0);
			RETURN -1;
		END

		-- Se arriviamo qui, abbiamo annullato l'appuntamento per cui, a meno di conversioni da WL dobbiamo incrementare i posti
		SET @mustIncrementPosti = 1;
		SELECT @idSchedule = ScheduleId,
			  @sogliaCancellazione = ScheduleCancellabileFinoAl
		FROM @tblUpdated
		
		-- Promuoviamo un utente dalla waiting list se è abilitata la WL e l'evento è FULL
		-- NOTA: non andiamo a verificare la presenza di utenti in WL se l'utente non è completo perché NON DOVREBBERO ESSERCENE
		IF EXISTS(SELECT 1 FROM @tblUpdated t WHERE t.WaitListAvailable = 1 AND PostiResidui = 0)
		BEGIN			
			EXEC [dbo].[internal_ListeAttesa_PromuoviToAppuntamento] 1, @idSchedule, @numRecordPromoted OUT
			IF @numRecordPromoted > 0
			BEGIN
				SET @mustIncrementPosti = 0; -- Non dobbiamo aumentare i Posti disponibili avendo convertito un utente in lista
			END
		END
		IF @mustIncrementPosti = 1
		BEGIN
			-- Riaggiungiamo un posto all'evento
			UPDATE Schedules
				SET PostiResidui = PostiResidui +1
			WHERE Id = @idSchedule
		END

		--Se siamo ancora nella fase in cui l'utente può cancellarsi ricevendo il riaccreditio, procediamo a farlo (sempre che ci sia un abbonamento da riaccreditare)
		IF @sogliaCancellazione > @dtOperazione AND @idAbbonamento IS NOT NULL
		BEGIN
			UPDATE AbbonamentiUtenti
				SET IngressiResidui = IngressiResidui +1
			WHERE Id = @idAbbonamento
			AND IngressiResidui IS NOT NULL -- se non sono previsti ingressi per questo abbonamento non facciamo riaccrediti
		END

	COMMIT
END