CREATE PROCEDURE [dbo].[Locations_List]
	@pIdCliente			INT,
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
			,[Colore]
			,[ImageUrl]
			,[IconUrl]
	FROM Locations
	WHERE IdCliente = @pIdCliente
	AND ((@pIncludeDeleted = 0) OR (DataCancellazione IS NULL))

	RETURN 0;
END