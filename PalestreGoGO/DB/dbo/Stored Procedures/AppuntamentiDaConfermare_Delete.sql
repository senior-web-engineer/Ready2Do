/*
Annulla un AppuntamentoDaConfermare
Ritorna:
 -1: Parametri non validi o prenotazione già confermata o cancellata
*/
CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Delete]
	@pIdCliente							INT,
	@pIdAppuntamentoDaConfermare		INT
AS
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @dtOperazione			DATETIME2 = SYSDATETIME(),
			@idAbbonamento			INT,
			@idSchedule				INT,
			@sogliaCancellazione	DATETIME2

	DECLARE @tblUpdated TABLE(Id INT, ScheduleId INT, UserId VARCHAR(100), TimeoutManagerPayload NVARCHAR(MAX))
	
	BEGIN TRANSACTION
	SET XACT_ABORT ON;
		
		-- Valorizziamo la DataCancellazione SOLO se non già valorizzata e se NON Confermato nel frattempo
		UPDATE AppuntamentiDaConfermare
			SET DataCancellazione = @dtOperazione
			OUTPUT inserted.Id, inserted.ScheduleId, inserted.UserId, inserted.TimeoutManagerPayload
				INTO @tblUpdated(Id, ScheduleId, UserId, TimeoutManagerPayload)
		WHERE Id = @pIdAppuntamentoDaConfermare
		AND IdCliente = @pIdCliente
		AND DataCancellazione IS NULL
		AND DataEsito IS NULL
		AND IdAppuntamento IS NULL

		IF @@ROWCOUNT = 0
		BEGIN
			RAISERROR(N'Parametri specificati non validi oppure prenotazione già annullata o confermata', 16, 0);
			RETURN -1;
		END
		
	COMMIT

	SELECT Id, ScheduleId, UserId, TimeoutManagerPayload FROM @tblUpdated
END