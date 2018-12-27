CREATE PROCEDURE [dbo].[Locations_Delete]
	@pIdCliente			INT,
	@pIdLocation		INT
AS
BEGIN

	UPDATE Locations
		SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pIdLocation
	AND IdCliente = @pIdCliente

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare la Location [%i] da eliminare per il Cliente [%i]' ,16, 0, @pIdLocation, @pIdCliente);
		RETURN -1;
	END

	RETURN 0;
END