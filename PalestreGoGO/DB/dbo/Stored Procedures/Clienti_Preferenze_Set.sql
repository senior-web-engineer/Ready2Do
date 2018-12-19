CREATE PROCEDURE [dbo].[Clienti_Preferenze_Set]
	@pIdCliente		int,
	@pKey			varchar(100),
	@pValue			nvarchar(MAX)
AS
BEGIN
	UPDATE ClientiPreferenze
		SET [Value] = @pValue
	WHERE IdCliente = @pIdCliente
	AND [Key] = @pKey 

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO ClientiPreferenze(IdCliente, [Key],[Value])	
			VALUES(@pIdCliente, @pKey, @pValue)
	END
END