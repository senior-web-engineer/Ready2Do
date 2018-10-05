CREATE PROCEDURE [dbo].[Utenti_ClientiFollowed]
	@pUserId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT	c.Id AS IdCliente,
			c.Nome,
			c.RagioneSociale,
			cu.DataCreazione as DataFollowing,
			CAST(CASE	
					WHEN av.NumAbbonamenti > 0 THEN 1
					ELSE 0 
				 END AS BIT) AS HasAbbonamentoValido
	FROM [ClientiUtenti] cu
		INNER JOIN [Clienti] c ON cu.IdCliente = c.Id
		OUTER APPLY (SELECT COUNT(*) FROM vAbbonamentiValidi av WHERE av.UserId = @pUserId AND av.IdCliente = c.Id) AS AV(NumAbbonamenti)
	where cu.IdUtente = @pUserId
END