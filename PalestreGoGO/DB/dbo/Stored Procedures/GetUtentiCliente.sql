CREATE PROCEDURE [dbo].[GetUtentiCliente]
	@pIdCliente INT 
AS
	SELECT  u.Id
		   ,u.UserName
		   ,u.FirstName
		   ,u.LastName
		   ,u.NumeroCellulare
		   ,u.Email
		   ,cu.DataCreazione AS DataAssociazione
		   ,au.Id AS IdAbbonamentoUtente
		   ,au.DataInizioValidita
		   ,au.IngressiResidui
		   ,au.Scadenza
		   ,au.ScadenzaCertificato
		   ,au.StatoPagamento
		   ,au.IdTipoAbbonamento
		   ,ta.Nome
		   ,ta.Costo
		   ,ta.DurataMesi
		   ,ta.MaxLivCorsi
		   ,ta.Nome
		   ,ta.NumIngressi
		   ,c.UrlRoute AS ClienteUrlRoute
	FROM [ClientiUtenti] cu
		INNER JOIN [security].[Users] u ON cu.IdUtente = u.Id
		INNER JOIN [Clienti] c ON c.Id = cu.IdCliente
		LEFT JOIN [AbbonamentiUtenti] au ON au.IdCliente = cu.IdCliente AND au.UserId = cu.IdUtente
		LEFT JOIN [TipologieAbbonamenti] ta ON ta.Id = au.IdTipoAbbonamento
	WHERE cu.IdCliente = @pIdCliente