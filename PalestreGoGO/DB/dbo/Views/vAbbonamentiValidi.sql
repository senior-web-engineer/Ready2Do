CREATE VIEW [dbo].[vAbbonamentiValidi]
	AS 
	SELECT   au.[Id]					AS IdAbbonamentoUtente
			,au.[IdCliente]				
			,au.[UserId]				
			,au.[IdTipoAbbonamento]		
			,au.[DataInizioValidita]	
			,au.[Scadenza]				
			,au.[IngressiResidui]		
			,ta.Costo
			,ta.DurataMesi
			,ta.MaxLivCorsi
			,ta.Nome
			,ta.NumIngressi --Ingressi inizialmente previsti
	FROM [AbbonamentiUtenti] au
		INNER JOIN [TipologieAbbonamenti] ta on ta.Id = au.IdTipoAbbonamento
	WHERE DataCancellazione IS NULL
	AND COALESCE(au.IngressiResidui, 1) > 0 
	AND Scadenza >=SYSDATETIME()
	--OR	  DATEADD(month, ta.DurataMesi, au.DataInizioValidita) >= SYSDATETIME()
