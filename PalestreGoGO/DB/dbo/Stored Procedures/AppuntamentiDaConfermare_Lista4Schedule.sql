CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Lista4Schedule]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT	adc.*,
			cu.*
	FROM vAppuntamentiDaConfermare adc
		-- escludiamo gli utenti cancellati 
		left join vClientiUtenti cu ON cu.UserIdClientiUtenti = adc.UserIdAppuntamentiDaConfermare 
									AND cu.IdClienteClientiUtenti = adc.IdClienteAppuntamentiDaConfermare 
									AND cu.DataCancellazioneClientiUtenti IS NULL 
	WHERE adc.IdClienteAppuntamentiDaConfermare = @pIdCliente
		AND adc.ScheduleIdAppuntamentiDaConfermare = @pIdSchedule
		AND ((COALESCE(@pIncludeDeleted,0) = 1) OR (adc.DataCancellazioneAppuntamentiDaConfermare IS NULL))
END