/*
Annulla un appuntamento
*/
CREATE PROCEDURE [dbo].[Appuntamenti_Delete]
	@pIdCliente				INT,
	@pIdAppuntamento		INT
AS
BEGIN
	SET XACT_ABORT ON;
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @dtOperazione			DATETIME2 = SYSDATETIME(),
			@idAbbonamento			INT,
			@idSchedule				INT,
			@sogliaCancellazione	DATETIME2

	DECLARE @tblUpdated TABLE(Id INT, ScheduleId INT, IdAbbonamento INT)
	DECLARE @tblScheduleUpdated TABLE(Id INT, CancellabileFinoAl DATETIME2);

	BEGIN TRANSACTION
		
		-- Valorizziamo la DataCancellazione (SOLO se non già valorizzata)
		UPDATE Appuntamenti
			SET DataCancellazione = @dtOperazione
			OUTPUT inserted.Id, inserted.ScheduleId, inserted.IdAbbonamento
				INTO @tblUpdated(Id, ScheduleId, IdAbbonamento)
		WHERE Id = @pIdAppuntamento
		AND IdCliente = @pIdCliente
		AND DataCancellazione IS NULL

		IF @@ROWCOUNT = 0
		BEGIN
			RAISERROR(N'Parametri specificati non validi o prenotazione già annullata', 16, 0);
			RETURN -1;
		END

		SELECT	@idSchedule = ScheduleId,
				@idAbbonamento = IdAbbonamento
		FROM @tblUpdated

		-- Riaggiungiamo un posto all'evento
		UPDATE Schedules
			SET PostiResidui = PostiResidui +1
			OUTPUT inserted.Id, inserted.CancellabileFinoAl INTO @tblScheduleUpdated(Id, CancellabileFinoAl)
		WHERE Id = @idSchedule

		SELECT @sogliaCancellazione = CancellabileFinoAl 
		FROM @tblScheduleUpdated

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