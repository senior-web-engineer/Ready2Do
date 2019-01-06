CREATE FUNCTION [dbo].[ClientePreferenzaGet]
(
	@pIdCliente		int,
	@pKey			varchar(100)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @result NVARCHAR(MAX)
	SELECT @result = [Value] FROM ClientiPreferenze WHERE IdCliente = @pIdCliente AND [Key] = @pKey
	RETURN @result;
END
