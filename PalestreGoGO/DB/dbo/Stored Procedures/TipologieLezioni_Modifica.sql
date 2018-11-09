CREATE PROCEDURE [dbo].[TipologieLezioni_Modifica]
	@pId							INT,
	@pIdCliente						INT,
	@pNome							NVARCHAR(100),
	@pDescrizione					NVARCHAR(500) = NULL,
	@pDurata						INT,
	@pMaxPartecipanti				INT = NULL,
	@pLimiteCancellazioneMinuti		SMALLINT = NULL,
	@pLivello						SMALLINT = 0
AS
BEGIN
	
	UPDATE TipologieLezioni
		SET IdCliente = @pIdCliente, 
			Nome = @pNome, 
			Descrizione = @pDescrizione, 
			Durata = @pDurata, 
			MaxPartecipanti = @pMaxPartecipanti, 
			LimiteCancellazioneMinuti = @pLimiteCancellazioneMinuti, 
			Livello = @pLivello			
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	
	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR('Parametri non validi!', 16, 0);
		RETURN -1;
	END

	RETURN 0
END
