CREATE PROCEDURE [dbo].[Schedules_Lookup]
	@pIdSchedules		[dbo].[udtListOfInt] READONLY,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT	s.*,
			(s.PostiDisponibiliSchedules - s.PostiResiduiSchedules) AS NumPrenotazioni
	FROM [vSchedulesFull] s
	WHERE ((COALESCE(@pIncludeDeleted, 0) = 1) OR (s.DataCancellazioneSchedules IS NULL))
END