CREATE VIEW [dbo].[vClientiProvisioned]
	AS 
	SELECT	C.Id,
			C.[Nome],
			C.[RagioneSociale],
			C.[Email],
			C.[NumTelefono],
			C.[Descrizione],
			C.[Indirizzo],		
			C.[Citta],		
			C.[ZipOrPostalCode],
			C.[Country],
			C.[Latitudine],
			C.[Longitudine],
			C.[DataCreazione],	
			C.[IdUserOwner],
			C.[CorrelationId],
			C.[DataProvisioning],
			C.[UrlRoute],
			C.[OrarioApertura],
			C.[StorageContainer],
			C.[IdStato],
			TC.Id AS IdTipologia,
			TC.Nome AS NomeTipologia,
			TC.Descrizione AS DescrizioneTipologia
	FROM [Clienti] C
		INNER JOIN [TipologieClienti] TC ON C.IdTipologia = TC.Id
	WHERE c.IdStato >= 3 --Solo provisioned
