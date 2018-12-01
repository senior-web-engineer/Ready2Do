CREATE PROCEDURE [dbo].[Clienti_Utenti_AbbonamentoList]
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pIncludeDeleted	bit = 0,
	@pIncludeExpired	bit = 0
AS
BEGIN
	SELECT 	 au.[Id]				
			,au.[IdCliente]			
			,au.[UserId]			
			,au.[IdTipoAbbonamento]	
			,au.[DataInizioValidita]
			,au.[Scadenza]			
			,au.[IngressiIniziali]	
			,au.[IngressiResidui]	
			,au.[Importo]			
			,au.[ImportoPagato]		
			,au.[DataCreazione]		
			,au.[DataCancellazione]	
			,ta.Nome AS NomeTipoAbbonamento
	FROM [AbbonamentiUtenti] au
		INNER JOIN  [TipologieAbbonamenti] ta ON au.IdTipoAbbonamento = ta.Id
	WHERE au.IdCliente = @pIdCliente
	AND au.UserId = @pUserId
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (au.DataCancellazione IS NULL))
	AND ((COALESCE(@pIncludeExpired, 0) = 1) OR (au.Scadenza < SYSDATETIME()))
END