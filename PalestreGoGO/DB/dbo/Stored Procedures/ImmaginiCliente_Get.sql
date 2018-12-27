CREATE PROCEDURE [dbo].[ImmaginiCliente_Get]
	@pIdCliente			INT,
	@pIdImmagine		INT,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SET @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0)

	SELECT *
	FROM ClientiImmagini ci
	INNER JOIN vClientiProvisioned c ON ci.IdCliente = c.Id
	WHERE ci.IdCliente = @pIdCliente
	AND ((@pIncludeDeleted = 1) OR (ci.DataCancellazione IS NULL))
END