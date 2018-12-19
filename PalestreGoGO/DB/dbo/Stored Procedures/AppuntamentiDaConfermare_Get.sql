CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Get]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIdAppuntamentoDaConf	int,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT *
	FROM AppuntamentiDaConfermare
	WHERE Id = @pIdAppuntamentoDaConf
	AND IdCliente = @pIdCliente
	AND ScheduleId = @pIdSchedule
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (DataCancellazione IS NULL))
END