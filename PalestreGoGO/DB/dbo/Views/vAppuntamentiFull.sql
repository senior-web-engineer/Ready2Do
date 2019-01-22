CREATE VIEW [dbo].[vAppuntamentiFull]
	AS 
 SELECT		   ap.*,
			   sc.*,
			   tl.*,
			   lo.*
		FROM vAppuntamenti ap
			INNER JOIN vSchedules sc ON ap.ScheduleIdAppuntamenti = sc.IdSchedules
			INNER JOIN vTipologieLezioni tl ON sc.IdTipoLezioneSchedules = tl.IdTipologieLezioni
			INNER JOIN vLocations lo ON sc.IdLocationSchedules = lo.IdLocations
