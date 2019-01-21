CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Lista4Schedule]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT	adc.Id,
			adc.IdCliente,
			adc.ScheduleId,
			adc.IdAppuntamento,
			adc.DataCreazione,
			adc.DataCancellazione,
			adc.DataCreazione,
			adc.DataEsito,
			adc.DataExpiration,
			adc.MotivoRifiuto,
			adc.UserId,
			cu.Cognome,
			cu.Nome,
			cu.UserDisplayName,
			cu.DataAggiornamento,
			cu.DataCancellazione AS DataCancellazioneUtente,
			cu.DataCreazione AS DataCreazioneUtente
	FROM AppuntamentiDaConfermare adc
		-- escludiamo gli utenti cancellati 
		left join ClientiUtenti cu ON cu.UserId = adc.UserId AND cu.IdCliente = adc.IdCliente AND cu.DataCancellazione IS NULL 
	WHERE adc.IdCliente = @pIdCliente
		AND adc.ScheduleId = @pIdSchedule
		AND ((COALESCE(@pIncludeDeleted,0) = 1) OR (adc.DataCancellazione IS NULL))
END