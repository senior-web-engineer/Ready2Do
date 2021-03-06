CREATE PROCEDURE [dbo].[ImmaginiCliente_Add]
	@pIdCliente		INT,
	@pIdTipoImmagine	INT, --Il TipoDiImmagine non è modificabile
	@pNome				NVARCHAR(100),
	@pAlt				NVARCHAR(100) = NULL,
	@pUrl				NVARCHAR(1000),
	@pThumbnailUrl		NVARCHAR(1000) = NULL,
	@pDescrizione		NVARCHAR(1000) = NULL,
	@pOrdinamento		INT,
	@pId				INT OUT
AS
BEGIN
	INSERT INTO ClientiImmagini([IdCliente], [IdTipoImmagine], [Nome], [Alt], [Url], [ThumbnailUrl], [Descrizione], [Ordinamento])
		SELECT @pIdCliente, @pIdTipoImmagine, @pNome, @pAlt, @pUrl, @pThumbnailUrl, @pDescrizione, @pOrdinamento

	SET @pId = SCOPE_IDENTITY();
END