CREATE PROCEDURE [dbo].[Clienti_Preferenze_Get]
	@pIdCliente		int,
	@pKey			varchar(100)
AS
BEGIN
	SELECT [Value] FROM  ClientiPreferenze
	WHERE IdCliente = @pIdCliente
	AND [Key] = @pKey 
END