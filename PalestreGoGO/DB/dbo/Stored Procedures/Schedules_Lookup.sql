CREATE PROCEDURE [dbo].[Schedules_Lookup]
	@pIdSchedules		[dbo].[udtListOfInt] READONLY,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT	s.*
	FROM [vSchedulesFull] s
		INNER JOIN @pIdSchedules ids ON s.IdSchedules = ids.Id
	WHERE ((COALESCE(@pIncludeDeleted, 0) = 1) OR (s.DataCancellazioneSchedules IS NULL))
END