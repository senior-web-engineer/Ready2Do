CREATE PROCEDURE [dbo].[Clienti_Utenti_AbbonamentoGet]
	@pIdCliente			int,
	@pIdAbbonamento		int
AS
BEGIN
	SELECT 	[Id]				
			,[IdCliente]			
			,[UserId]			
			,[IdTipoAbbonamento]	
			,[DataInizioValidita]
			,[Scadenza]			
			,[IngressiIniziali]	
			,[IngressiResidui]	
			,[Importo]			
			,[ImportoPagato]		
			,[DataCreazione]		
			,[DataCancellazione]	
	FROM [AbbonamentiUtenti]
	WHERE IdCliente = @pIdCliente
	AND Id = @pIdAbbonamento
END