CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Get]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pIdAppuntamentoDaConf	int,
	@pIncludeDeleted		bit = 0
AS
BEGIN
	DECLARE @MAX_LIV_CORSO SMALLINT = 32767
	
	SELECT	adc.*
			,T.HasAbbonamento AS CanBeconfirmedAppuntamentiDaConfermare
	FROM vAppuntamentiDaConfermareFull adc
		INNER JOIN TipologieLezioni tl ON adc.IdTipoLezioneSchedules = tl.Id
		-- Calcoliamo se l'appuntamento può essere confermato o meno andando a verificare l'esistenza di un abbonamento
		OUTER APPLY [dbo].[ExistsAbbonamentoValido](@pIdCliente, adc.UserIdAppuntamentiDaConfermare, tl.Livello) AS T
	WHERE adc.IdAppuntamentiDaConfermare = @pIdAppuntamentoDaConf
	AND adc.IdClienteAppuntamentiDaConfermare = @pIdCliente
	AND adc.ScheduleIdAppuntamentiDaConfermare = @pIdSchedule
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (adc.DataCancellazioneAppuntamentiDaConfermare IS NULL))
END