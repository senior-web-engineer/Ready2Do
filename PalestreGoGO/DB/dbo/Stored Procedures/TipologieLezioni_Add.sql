CREATE PROCEDURE [dbo].[TipologieLezioni_Add]
	@pIdCliente						INT,
	@pNome							NVARCHAR(100),
	@pDescrizione					NVARCHAR(500) = NULL,
	@pDurata						INT,
	@pMaxPartecipanti				INT = NULL,
	@pLimiteCancellazioneMinuti		SMALLINT = NULL,
	@pLivello						SMALLINT = 0,
	@pPrezzo						DECIMAL(10,2) = NULL,
	@pId							INT OUTPUT
AS
BEGIN

	INSERT INTO TipologieLezioni(IdCliente, Nome, Descrizione, Durata, MaxPartecipanti, LimiteCancellazioneMinuti, Livello, Prezzo)
		VALUES (@pIdCliente, @pNome, @pDescrizione, @pDurata, @pMaxPartecipanti, @pLimiteCancellazioneMinuti, @pLivello, @pPrezzo);

	SET @pId = SCOPE_IDENTITY()
	
	RETURN 0
END
