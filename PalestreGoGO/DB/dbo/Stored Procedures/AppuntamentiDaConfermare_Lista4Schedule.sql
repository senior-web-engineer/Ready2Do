CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Lista4Schedule]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	SELECT	adc.*,
			T.HasAbbonamento AS CanBeconfirmedAppuntamentiDaConfermare,
			cu.*
	FROM vAppuntamentiDaConfermareFull adc
		inner join TipologieLezioni tl ON adc.IdTipoLezioneSchedules = tl.Id
		-- escludiamo gli utenti cancellati 
		left join vClientiUtenti cu ON cu.UserIdClientiUtenti = adc.UserIdAppuntamentiDaConfermare 
									AND cu.IdClienteClientiUtenti = adc.IdClienteAppuntamentiDaConfermare 
									AND cu.DataCancellazioneClientiUtenti IS NULL 
		outer apply [dbo].[ExistsAbbonamentoValido](@pIdCliente, adc.UserIdAppuntamentiDaConfermare, tl.Livello) AS T
	WHERE adc.IdClienteAppuntamentiDaConfermare = @pIdCliente
		AND adc.ScheduleIdAppuntamentiDaConfermare = @pIdSchedule
		AND ((COALESCE(@pIncludeDeleted,0) = 1) OR (adc.DataCancellazioneAppuntamentiDaConfermare IS NULL))
END