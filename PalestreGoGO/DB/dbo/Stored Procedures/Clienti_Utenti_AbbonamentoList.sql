CREATE PROCEDURE [dbo].[Clienti_Utenti_AbbonamentoList]
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pIncludeDeleted	bit = 0,
	@pIncludeExpired	bit = 0
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
	AND UserId = @pUserId
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (DataCancellazione IS NULL))
	AND ((COALESCE(@pIncludeExpired, 0) = 1) OR (Scadenza < SYSDATETIME()))
END