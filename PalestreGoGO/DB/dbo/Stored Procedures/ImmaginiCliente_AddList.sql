CREATE PROCEDURE [dbo].[ImmaginiCliente_AddList]
	@pIdCliente		INT,
	@pImmagini		[udtListOfImmagini] READONLY
AS
BEGIN
	INSERT INTO ClientiImmagini([IdCliente], [IdTipoImmagine], [Nome], [Alt], [Url], [ThumbnailUrl], [Descrizione], [Ordinamento])
		SELECT @pIdCliente, I.IdTipoImmagine, I.Nome, I.Alt, I.[Url], I.ThumbnailUrl, I.Descrizione, I.Ordinamento
		FROM @pImmagini I
END