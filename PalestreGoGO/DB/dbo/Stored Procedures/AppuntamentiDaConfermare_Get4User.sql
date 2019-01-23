CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Get4User]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pUserId				varchar(100),
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT *
	FROM vAppuntamentiDaConfermare
	WHERE IdClienteAppuntamentiDaConfermare = @pIdCliente
	AND ScheduleIdAppuntamentiDaConfermare = @pIdSchedule
	AND UserIdAppuntamentiDaConfermare =  @pUserId
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (DataCancellazioneAppuntamentiDaConfermare IS NULL))
END