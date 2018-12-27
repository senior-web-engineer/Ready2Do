CREATE PROCEDURE [dbo].[ImmaginiCliente_Lista]
	@pIdCliente			INT,
	@pIdTipoImmagine	INT = NULL,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SET @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0)

	SELECT *
	FROM ClientiImmagini ci
	INNER JOIN vClientiProvisioned c ON ci.IdCliente = c.Id
	WHERE ci.IdCliente = @pIdCliente
	AND ((@pIdTipoImmagine IS NULL) OR (ci.IdTipoImmagine= @pIdTipoImmagine))
	AND ((@pIncludeDeleted = 1) OR (ci.DataCancellazione IS NULL))
END