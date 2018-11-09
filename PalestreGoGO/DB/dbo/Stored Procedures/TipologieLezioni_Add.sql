CREATE PROCEDURE [dbo].[TipologieLezioni_Add]
	@pIdCliente						INT,
	@pNome							NVARCHAR(100),
	@pDescrizione					NVARCHAR(500) = NULL,
	@pDurata						INT,
	@pMaxPartecipanti				INT = NULL,
	@pLimiteCancellazioneMinuti		SMALLINT = NULL,
	@pLivello						SMALLINT = 0,
	@pId							INT OUTPUT
AS
BEGIN

	INSERT INTO TipologieLezioni(IdCliente, Nome, Descrizione, Durata, MaxPartecipanti, LimiteCancellazioneMinuti, Livello)
		VALUES (@pIdCliente, @pNome, @pDescrizione, @pDurata, @pMaxPartecipanti, @pLimiteCancellazioneMinuti, @pLivello);

	SET @pId = SCOPE_IDENTITY()
	
	RETURN 0
END
