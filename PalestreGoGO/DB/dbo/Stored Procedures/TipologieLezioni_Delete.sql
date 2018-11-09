CREATE PROCEDURE [dbo].[TipologieLezioni_Delete]
	@pId			INT,
	@pIdCliente		INT
AS
BEGIN
	UPDATE TipologieLezioni
		SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	
	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR('Parametri non validi!', 16, 0);
		RETURN -1;
	END
	
	RETURN 0;
END