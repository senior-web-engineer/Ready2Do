CREATE PROCEDURE [dbo].[Locations_List]
	@pIdCliente			INT,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SET @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0);
	
	SELECT   *
	FROM vLocations
	WHERE IdClienteLocations = @pIdCliente
	AND ((@pIncludeDeleted = 0) OR (DataCancellazioneLocations IS NULL))

	RETURN 0;
END