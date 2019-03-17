CREATE PROCEDURE [dbo].[ImmaginiCliente_Modifica]
	@pIdCliente			INT,
	@pIdImmagine		INT,
	--@pIdTipoImmagine	INT, --Il TipoDiImmagine non è modificabile
	@pNome				NVARCHAR(100),
	@pAlt				NVARCHAR(100) = NULL,
	@pUrl				NVARCHAR(1000),
	@pThumbnailUrl		NVARCHAR(1000) = NULL,
	@pDescrizione		NVARCHAR(1000) = NULL,
	@pOrdinamento		INT
AS
BEGIN
	UPDATE ClientiImmagini
		SET Nome = @pNome,
			Alt = @pAlt,
			[Url] = @pUrl,
			ThumbnailUrl = @pThumbnailUrl,
			Descrizione = @pDescrizione,
			Ordinamento = @pOrdinamento
	WHERE Id = @pIdImmagine
	AND IdCliente = @pIdCliente
	AND DataCancellazione IS NULL

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare l''immagine [%i] per il Cliente [%i]', 16 ,1, @pIdImmagine, @pIdCliente);
		RETURN -1;
	END
END