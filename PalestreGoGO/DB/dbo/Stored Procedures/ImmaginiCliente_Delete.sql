CREATE PROCEDURE [dbo].[ImmaginiCliente_Delete]
	@pIdCliente		INT,
	@pIdImmagine	INT
AS
BEGIN
	UPDATE ClientiImmagini
		SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pIdImmagine
	AND IdCliente = @pIdCliente
	AND DataCancellazione IS NULL

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile eliminare l''immagine [%i] per il Cliente [%i]', 16 ,1, @pIdImmagine, @pIdCliente);
		RETURN -1;
	END
END