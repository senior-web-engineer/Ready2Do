CREATE VIEW [dbo].[vAppuntamentiDaConfermareFull]
	AS 
 SELECT		 adc.*,
			 sc.*
		FROM vAppuntamentiDaConfermare adc
			INNER JOIN vSchedules sc ON adc.ScheduleIdAppuntamentiDaConfermare = sc.IdSchedules AND adc.IdClienteAppuntamentiDaConfermare = sc.IdClienteSchedules
