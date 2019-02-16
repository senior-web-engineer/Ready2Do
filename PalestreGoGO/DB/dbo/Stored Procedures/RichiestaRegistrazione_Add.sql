/*
	Inserisce un nuovo record nella tabella RichiesteRegistrazione e genera il codice di conferma randomicamente.
	Se per l'utente esiste già una richiesta pendente viene cancellata e ne viene creata una nuova (il vecchio codice non sarà più valido)
*/
CREATE PROCEDURE [dbo].[RichiestaRegistrazione_Add]
	@pUsername			VARCHAR(500),
	@pExpiration		DATETIME2(2) = NULL,
	@pCorrelationId		UNIQUEIDENTIFIER = NULL,
	@pRefereer			INT = NULL,
	@pUserCode			VARCHAR(1000) OUTPUT
AS
BEGIN
	DECLARE @id INT,
			@dataOperazione DATETIME2 = SYSDATETIME(),
			@dataConferma DATETIME2 = NULL,
			@random	 VARBINARY(100);
	
	-- Se esiste una richiesta ancora valida ritorniamo il codice di quella
	SELECT @pUserCode = UserCode,
		   @dataConferma = DataConferma
	FROM RichiesteRegistrazione
		WHERE Username = @pUsername
		AND Expiration > SYSDATETIME()
		AND DataCancellazione IS NULL
	
	IF @dataConferma IS NOT NULL
	BEGIN
		RAISERROR ('Utente già confermato', 16, 0);
		RETURN -1;
	END

	-- Se non abbiamo trovato una richiesta valida, ne creiamo una nuova	
	IF @pUserCode IS NULL
	BEGIN
		SELECT	@random = CRYPT_GEN_RANDOM(100),
				@pCorrelationId = COALESCE(@pCorrelationId, NEWID()),
				@pExpiration = COALESCE(@pExpiration, DATEADD(DAY, 1, SYSDATETIME()))
		
		-- Calcoliamo l'HASH SHA2-256 (32 Bytes) dei byte random generati precedentemente e li convertiamo in BASE64 usando OPENJSON
		SELECT @pUserCode = col FROM OPENJSON(
		(
			SELECT col FROM (SELECT  HASHBYTES('SHA2_256', @random) AS col) T
			FOR JSON AUTO
		)
		) WITH (col VARCHAR(MAX))

		-- Inseriamo una richiesta SOLO se non ce n'è una già valida
		INSERT INTO RichiesteRegistrazione(CorrelationId, UserCode, Username, Expiration, Refereer)
			SELECT @pCorrelationId, @pUserCode, @pUsername, @pExpiration, @pRefereer
				WHERE NOT EXISTS(SELECT * FROM RichiesteRegistrazione 
								WHERE Username = @pUsername AND DataCancellazione IS NULL AND Expiration > @dataOperazione )
	
		IF @@ROWCOUNT = 0
		BEGIN
			RAISERROR (N'Impossibile inserire una nuva richiesta di registrazione', 16, 1)
			RETURN -1
		END

		SET @id =SCOPE_IDENTITY()
	
	END
END