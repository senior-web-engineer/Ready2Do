CREATE PROCEDURE [dbo].[Clienti_Utenti_AbbonamentoList]
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pIdEvento			int = null,
	@pIncludeDeleted	bit = 0,
	@pIncludeExpired	bit = 0,
	@pMaxItems			int = 2147483647 --default MaxInt
AS
BEGIN
	SET @pMaxItems = COALESCE(@pMaxItems, 2147483647);

	SELECT 	TOP(@pMaxItems) 
			au.[Id]				
			,au.[IdCliente]			
			,au.[UserId]			
			,au.[DataInizioValidita]
			,au.[Scadenza]			
			,au.[IngressiIniziali]	
			,au.[IngressiResidui]	
			,au.[Importo]			
			,au.[ImportoPagato]		
			,au.[DataCreazione]		
			,au.[DataCancellazione]	
			,au.[IdTipoAbbonamento]	
			,ta.[Nome] AS NomeTipoAbobonamento		
			,ta.[DurataMesi]
			,ta.[NumIngressi]
			,ta.[Costo]		
			,ta.[MaxLivCorsi]
			,ta.ValidoDal
			,ta.ValidoFinoAl
			,ta.DataCreazione AS DataCreazioneTipologiaAbbonamento
			,ta.DataCancellazione AS DataCancellazioneTipologiaAbbonamento
	FROM [AbbonamentiUtenti] au
		INNER JOIN  [TipologieAbbonamenti] ta ON au.IdTipoAbbonamento = ta.Id
	WHERE au.IdCliente = @pIdCliente
	AND au.UserId = @pUserId
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (au.DataCancellazione IS NULL))
	AND ((COALESCE(@pIncludeExpired, 0) = 1) OR (au.Scadenza < SYSDATETIME()))
	--Se specificato l'id di un evento, andiamo a filtrare i soli abbonamenti "compatibili"
	AND ((@pIdEvento IS NULL) OR EXISTS(SELECT 1 FROM Schedules s 
										INNER JOIN TipologieLezioni tl ON s.IdTipoLezione = tl.Id
										WHERE s.Id = @pIdEvento
										AND tl.Livello <= ta.MaxLivCorsi))
	ORDER BY au.DataCreazione DESC -- gli ultimi li ritorniamo per primi
END