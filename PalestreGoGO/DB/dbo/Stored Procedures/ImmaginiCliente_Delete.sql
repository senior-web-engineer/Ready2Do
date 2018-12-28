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
	-- Ritorniamo l'immagine cancellata. Ci serve (glo Url) per far cancellare al chiamante i dati dal Blob Storage
	SELECT * FROM ClientiImmagini WHERE Id = @pIdImmagine AND IdCliente = @pIdCliente
END