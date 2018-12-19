CREATE PROCEDURE [dbo].[Clienti_Preferenze_List]
	@pIdCliente		int
AS
BEGIN
	SELECT [Key], [Value] FROM  ClientiPreferenze
	WHERE IdCliente = @pIdCliente
END