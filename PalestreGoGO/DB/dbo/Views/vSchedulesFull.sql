CREATE VIEW [dbo].[vSchedulesFull]
AS 
SELECT	 s.*
		,l.*
		,tl.*
FROM vSchedules s
	INNER JOIN vTipologieLezioni tl ON tl.IdTipologieLezioni = s.IdTipoLezioneSchedules AND tl.IdClienteTipologieLezioni = s.IdClienteSchedules
	INNER JOIN vLocations l ON l.IdLocations = s.IdLocationSchedules AND l.IdClienteLocations = s.IdClienteSchedules
