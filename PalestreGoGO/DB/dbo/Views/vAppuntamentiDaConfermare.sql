CREATE VIEW [dbo].[vAppuntamentiDaConfermare]
	AS 
	SELECT 
		 Id						AS IdAppuntamentiDaConfermare
		,IdCliente				AS IdClienteAppuntamentiDaConfermare
		,UserId					AS UserIdAppuntamentiDaConfermare
		,ScheduleId				AS ScheduleIdAppuntamentiDaConfermare
		,DataCreazione			AS DataCreazioneAppuntamentiDaConfermare
		,DataExpiration			AS DataExpirationAppuntamentiDaConfermare
		,DataEsito				AS DataEsitoAppuntamentiDaConfermare
		,IdAppuntamento			AS IdAppuntamentoAppuntamentiDaConfermare
		,MotivoRifiuto			AS MotivoRifiutoAppuntamentiDaConfermare
		,DataCancellazione		AS DataCancellazioneAppuntamentiDaConfermare
		,TimeoutManagerPayload	AS TimeoutManagerPayloadAppuntamentiDaConfermare
	FROM AppuntamentiDaConfermare
