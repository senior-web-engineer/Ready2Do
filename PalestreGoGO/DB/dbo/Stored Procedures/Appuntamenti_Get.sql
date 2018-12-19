CREATE PROCEDURE [dbo].[Appuntamenti_Get]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIdAppuntamento		int,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT *
	FROM Appuntamenti
	WHERE IdCliente = @pIdCliente
	AND ScheduleId = @pIdSchedule
	AND Id = @pIdAppuntamento
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (DataCancellazione IS NULL))
END