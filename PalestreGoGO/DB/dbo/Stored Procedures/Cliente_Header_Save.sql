/*
Nota: Può esserci una sola immagine di tipo SFONDO per un Cliente
*/
CREATE PROCEDURE [dbo].[Cliente_Header_Save]
	@pIdCliente			int = 0,
	@pHeaderImageUrl	varchar(2000)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM Clienti WHERE Id = @pIdCliente)
	BEGIN
		RETURN -1; -- Cliente Iniesistente
	END

	DECLARE @tblOut TABLE(Id INT)
	DECLARE @idImage INT;

	UPDATE CI 
		SET [Url] = @pHeaderImageUrl
		OUTPUT inserted.Id INTO @tblOut(Id)
	FROM ClientiImmagini ci 
		INNER JOIN TipologieImmagini ti ON ci.IdTipoImmagine = ti.Id
	WHERE IdCliente = @pIdCliente
	AND ti.Codice = 'SFONDO'
	
	SELECT @idImage = Id FROM @tblOut;

	--Se non ho aggiornato nessuna immagine ==> la inserisco
	IF @idImage IS NULL
	BEGIN
		INSERT INTO ClientiImmagini (IdCliente, IdTipoImmagine, Nome, Alt, [Url], Descrizione)
			SELECT @pIdCliente, Id, 'Header', NULL, @pHeaderImageUrl, NULL
			FROM TipologieImmagini WHERE Codice = 'SFONDO'
		SET @idImage = SCOPE_IDENTITY();
	END

	--Ritorniamo l'immagine
	SELECT IdCliente, IdTipoImmagine, Nome, Alt, [Url], Descrizione, Ordinamento
	FROM ClientiImmagini
	WHERE Id = @idImage

END