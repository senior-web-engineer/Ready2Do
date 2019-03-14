CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Get4User]
	@pIdCliente				int,
	@pIdSchedule			int = null,
	@pUserId				varchar(100),
	@pIncludeDeleted		bit = 0,
	@pIncludeExpired		bit = 0
AS
BEGIN
	SELECT	adc.*,
			T.HasAbbonamento AS CanBeconfirmedAppuntamentiDaConfermare
	FROM vAppuntamentiDaConfermareFull adc
		inner join TipologieLezioni tl ON adc.IdTipoLezioneSchedules = tl.Id
		outer apply [dbo].[ExistsAbbonamentoValido](@pIdCliente, adc.UserIdAppuntamentiDaConfermare, tl.Livello) AS T
	WHERE adc.IdClienteAppuntamentiDaConfermare = @pIdCliente
	AND adc.DataEsitoAppuntamentiDaConfermare IS NULL --escludiamo tutti quelli cone sito (confermati o rifiutati)
	AND ((@pIdSchedule IS NULL) OR (adc.ScheduleIdAppuntamentiDaConfermare = @pIdSchedule))
	AND adc.UserIdAppuntamentiDaConfermare =  @pUserId
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (adc.DataCancellazioneAppuntamentiDaConfermare IS NULL))
	AND ((COALESCE(@pIncludeExpired, 0) = 1) OR (adc.DataExpirationAppuntamentiDaConfermare > SYSDATETIME()))
END