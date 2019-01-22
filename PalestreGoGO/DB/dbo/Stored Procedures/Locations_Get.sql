CREATE PROCEDURE [dbo].[Locations_Get]
	@pIdCliente			INT,
	@pIdLocation		INT,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SET @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0);
	
	SELECT  *
	FROM vLocations
	WHERE IdClienteLocations = @pIdCliente
	AND IdLocations = @pIdLocation
	AND ((@pIncludeDeleted = 0) OR (DataCancellazioneLocations IS NULL))

	RETURN 0;
END