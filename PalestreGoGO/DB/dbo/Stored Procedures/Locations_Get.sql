CREATE PROCEDURE [dbo].[Locations_Get]
	@pIdCliente			INT,
	@pIdLocation		INT,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SET @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0);
	
	SELECT   [Id]
			,[IdCliente]
			,[Nome]
			,[Descrizione]
			,[CapienzaMax]
			,[DataCreazione]
			,[DataCancellazione]
	FROM Locations
	WHERE IdCliente = @pIdCliente
	AND Id = @pIdLocation
	AND ((@pIncludeDeleted = 0) OR (DataCancellazione IS NULL))

	RETURN 0;
END