CREATE VIEW [dbo].[vAbbonamentiUtenti]
AS 	
SELECT
	 [Id]						AS IdAbbonamentiUtenti
	,[IdCliente]				AS IdClienteAbbonamentiUtenti
	,[UserId]					AS UserIDAbbonamentiUtenti
	,[IdTipoAbbonamento]		AS IdTipoAbbonamentoAbbonamentiUtenti
	,[DataInizioValidita]		AS DataInizioValiditaAbbonamentiUtenti
	,[Scadenza]					AS ScadenzaAbbonamentiUtenti
	,[IngressiIniziali]			AS IngressiInizialiAbbonamentiUtenti	
	,[IngressiResidui]			AS IngressiResiduiAbbonamentiUtenti
	,[Importo]					AS ImportoAbbonamentiUtenti
	,[ImportoPagato]			AS ImportoPagatoAbbonamentiUtenti
	,[DataCreazione]			AS DataCreazioneAbbonamentiUtenti
	,[DataCancellazione]		AS DataCancellazioneAbbonamentiUtenti
FROM [AbbonamentiUtenti]		