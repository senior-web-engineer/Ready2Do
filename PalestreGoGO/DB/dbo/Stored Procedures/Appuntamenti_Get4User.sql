CREATE PROCEDURE [dbo].[Appuntamenti_Get4User]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pUserId				varchar(100),
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT *
	FROM Appuntamenti
	WHERE IdCliente = @pIdCliente
	AND ScheduleId = @pIdSchedule
	AND UserId =  @pUserId
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (DataCancellazione IS NULL))
END