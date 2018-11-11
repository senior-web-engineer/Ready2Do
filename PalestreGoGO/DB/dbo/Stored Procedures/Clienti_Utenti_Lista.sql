CREATE PROCEDURE [dbo].[Clienti_Utenti_Lista]
	@pIdCliente INT 
AS
BEGIN
	SELECT  cu.DataCreazione AS DataAssociazione
		   ,cu.IdUtente AS UserId
		   ,cu.NominativoUser AS NominativoUser
		   ,cu.UserDisplayName	AS UserDisplayName
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
		INNER JOIN [Clienti] c ON c.Id = cu.IdCliente
		LEFT JOIN [AbbonamentiUtenti] au ON au.IdCliente = cu.IdCliente AND au.UserId = cu.IdUtente
		LEFT JOIN [TipologieAbbonamenti] ta ON ta.Id = au.IdTipoAbbonamento
	WHERE cu.IdCliente = @pIdCliente
END