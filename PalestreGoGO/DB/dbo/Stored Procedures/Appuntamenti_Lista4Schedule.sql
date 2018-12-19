CREATE PROCEDURE [dbo].[Appuntamenti_Lista4Schedule]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIncludeConfermati		bit,
	@pIncludeNonConfermati	bit,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	IF COALESCE(@pIncludeConfermati, 0) = 1
	BEGIN
		SELECT * 
		FROM Appuntamenti 
		WHERE IdCliente = @pIdCliente
		 AND ScheduleId = @pIdSchedule
		 AND ((COALESCE(@pIncludeDeleted,0) = 1) OR ( DataCancellazione IS NULL))
	END

	IF COALESCE(@pIncludeNonConfermati, 0) = 1
	BEGIN
		SELECT * 
		FROM AppuntamentiDaConfermare 
		WHERE IdCliente = @pIdCliente
		 AND ScheduleId = @pIdSchedule
		 AND ((COALESCE(@pIncludeDeleted,0) = 1) OR ( DataCancellazione IS NULL))
	END
END