CREATE VIEW [dbo].[vListeAttesa]
AS 
	SELECT 
	Id						AS IdListeAttesa
	,IdCliente				AS IdClienteListeAttesa
	,IdSchedule				AS IdScheduleListeAttesa
	,UserId					AS UserIdListeAttesa
	,IdAbbonamento			AS IdAbbonamentoListeAttesa
	,DataCreazione			AS DataCreazioneListeAttesa
	,DataScadenza			AS DataScadenzaListeAttesa
	,DataConversione		AS DataConversioneListeAttesa
	,DataCancellazione		AS DataCancellazioneListeAttesa
	,CausaleCancellazione	AS CausaleCancellazioneListeAttesa
	FROM ListeAttesa
