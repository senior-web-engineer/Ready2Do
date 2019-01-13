CREATE PROCEDURE [dbo].[Utenti_ClientiFollowed]
	@pUserId VARCHAR(100)
AS
BEGIN
	SELECT	c.Id AS IdCliente,
			c.Nome,
			c.RagioneSociale,
			cu.DataCreazione as DataFollowing	
	FROM [ClientiUtenti] cu
		INNER JOIN [Clienti] c ON cu.IdCliente = c.Id
	where cu.UserId = @pUserId
END