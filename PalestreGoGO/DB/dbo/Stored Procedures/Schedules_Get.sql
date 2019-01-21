CREATE PROCEDURE [dbo].[Schedules_Get]
	@pId				INT,
	@pIdCliente			INT,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT	
		s.*		
	FROM [vSchedulesFull] s
	WHERE s.IdSchedules = @pId
	AND s.IdClienteSchedules = @pIdCliente
	AND ((COALESCE(@pIncludeDeleted, 0) =1) OR ( s.DataCancellazioneSchedules IS NULL))
END