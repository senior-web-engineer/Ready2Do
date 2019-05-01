/*
	Inserisce un nuovo record nella tabella RichiesteRegistrazione e genera il codice di conferma randomicamente.
	Se per l'utente esiste già una richiesta pendente valida, viene ritornato il codice di quella altrimenti ne viene creata una nuova.
	NOTA:
	Il parametro @pCorrelationId serve quando si tratta di Richieste relativi a Clienti, in questo caso dobbiamo correlare la richiesta con il Cliente
	per poter cambiare lo stato del Cliente quando viene completata la richiesta.
    ~~~~~~~~~ HISTORY ~~~~~~~~~
	20190501: Rilassiamo il controllo sull'esistenza di una precedente richiesta già confermata come workaround per l'esistenza di utenti diversi 
			  con la stessa email (social e locali)
*/
CREATE PROCEDURE [dbo].[RichiestaRegistrazione_Add]
	@pUsername			VARCHAR(500),
	@pExpiration		DATETIME2(2) = NULL,
	@pCorrelationId		UNIQUEIDENTIFIER = NULL, -- ci serve se
	@pRefereer			INT = NULL,
	@pUserCode			VARCHAR(1000) OUTPUT
AS
BEGIN
	DECLARE @id INT,
			@userCode			VARCHAR(1000),
			@dataOperazione		DATETIME2 = SYSDATETIME(),
			@dataConferma		DATETIME2 = NULL,
			@dataExpirtation	DATETIME2,
			@dataCancellazione	DATETIME2,
			@oldCorrelation		UNIQUEIDENTIFIER,
			@random	 VARBINARY(100);
	
	-- Verifichiamo se esiste già una richiesta per lo username
	SELECT TOP 1
		   @userCode = UserCode,
		   @dataConferma = DataConferma,
		   @dataCancellazione = DataCancellazione,
		   @dataExpirtation = Expiration,
		   @oldCorrelation = CorrelationId
	FROM RichiesteRegistrazione
		WHERE Username = @pUsername
	ORDER BY DataRichiesta DESC

	-- Se abbiamo trovato una richiesta ancora valida, ritorniamo il codice
	IF @dataCancellazione IS NULL AND @dataExpirtation > SYSDATETIME()
	BEGIN
		SET @pUserCode = @userCode
		RETURN 1;
	END
	-- Se non abbiamo trovato una richiesta valida, ne creiamo una nuova
	-- se il correlationId è stato pasato usiamo quello, altrimenti usiamo quello della vecchia richiesta
	ELSE
	BEGIN
		SELECT	@random = CRYPT_GEN_RANDOM(100),
				@pCorrelationId = COALESCE(@pCorrelationId, @oldCorrelation),
				@pExpiration = COALESCE(@pExpiration, DATEADD(DAY, 1, SYSDATETIME()))
		
		-- Calcoliamo l'HASH SHA2-256 (32 Bytes) dei byte random generati precedentemente e li convertiamo in BASE64 usando OPENJSON
		SELECT @pUserCode = col FROM OPENJSON(
		(
			SELECT col FROM (SELECT  HASHBYTES('SHA2_256', @random) AS col) T
			FOR JSON AUTO
		)
		) WITH (col VARCHAR(MAX))

		-- Inseriamo una richiesta SOLO se non ce n'è una già valida
		MERGE RichiesteRegistrazione T
		USING ( SELECT @pCorrelationId, 
					  @pUserCode, 
					  @pUsername, 
					  @pExpiration, 
					  @pRefereer) AS S(CorrelationId, UserCode, Username, Expiration, Refereer)
			 ON T.UserName = S.UserName
		WHEN MATCHED AND T.DataCancellazione IS NULL 
				THEN UPDATE SET T.Expiration = S.Expiration,
							    T.CorrelationId = S.CorrelationId,
								T.UserCode = S.UserCode
		WHEN NOT MATCHED BY TARGET THEN INSERT (CorrelationId, UserCode, Username, Expiration, Refereer)
										VALUES (CorrelationId, UserCode, Username, Expiration, Refereer);

		--INSERT INTO RichiesteRegistrazione(CorrelationId, UserCode, Username, Expiration, Refereer)
		--	SELECT @pCorrelationId, @pUserCode, @pUsername, @pExpiration, @pRefereer
		--		WHERE NOT EXISTS(SELECT * FROM RichiesteRegistrazione 
		--						WHERE Username = @pUsername AND DataCancellazione IS NULL AND Expiration > @dataOperazione )
	
		IF @@ROWCOUNT = 0
		BEGIN
			RAISERROR (N'Impossibile inserire una nuva richiesta di registrazione', 16, 1)
			RETURN -1
		END

		SET @id =SCOPE_IDENTITY()
	
	END
END