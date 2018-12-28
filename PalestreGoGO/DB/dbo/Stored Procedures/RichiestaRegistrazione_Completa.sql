/*
Conferma una richiesta di Registrazione.
Il parametro @pEsitoConferma contiene l'esito della conferma e può assumere uno dei seguenti valori:
	 1 -> OK
	-1 -> Codice e/o Username non valido
	-2 -> Richiesta già confermata
	-3 -> Richiesta scaduta
*/
CREATE PROCEDURE [dbo].[RichiestaRegistrazione_Completa]
	@pUserCode			VARCHAR(1000),
	@pUserName			VARCHAR(500),
	@pEsitoConferma		INT OUT,
	@pIdCliente			INT OUT,
	@pIdRefereer		INT OUT
AS
BEGIN
	DECLARE @dtOp DATETIME2 = SYSDATETIME()
	DECLARE @dtExpiration DATETIME2;
	DECLARE @dtConferma	DATETIME2;
	DECLARE @STATO_PROVISIONED TINYINT = 3;
	DECLARE @STATO_CONFERMATO TINYINT = 10;
	DECLARE @tblRichiesta TABLE (CorrelationId	UNIQUEIDENTIFIER,
								 Refereer		INT,
								 IdCliente		INT);
	BEGIN TRANSACTION
	SET XACT_ABORT ON;

	SELECT  @dtExpiration = Expiration,
			@dtConferma = DataConferma
	FROM [RichiesteRegistrazione] WITH (UPDLOCK)
	WHERE UserName = @pUserName
	AND UserCode = @pUserCode
	AND DataCancellazione IS NULL

	IF @dtExpiration IS NULL
	BEGIN
		SET @pEsitoConferma = -1;
		RETURN 0;
	END

	IF @dtConferma IS NOT NULL
	BEGIN
		SET @pEsitoConferma = -2;
		RETURN 0;
	END

	IF @dtExpiration < @dtOp
	BEGIN
		SET @pEsitoConferma = -3
		RETURN 0;
	END

	UPDATE  R
		SET R.DataConferma = @dtOp
	OUTPUT inserted.CorrelationId, inserted.Refereer, c.Id INTO @tblRichiesta (CorrelationId, Refereer, IdCliente)
	FROM [RichiesteRegistrazione] R
		LEFT JOIN [Clienti] C ON C.CorrelationId = R.CorrelationId
	WHERE R.Username = @pUserName
	AND R.UserCode = @pUserCode
	AND R.Expiration > @dtOp
	AND R.DataConferma IS NULL 
	AND R.DataCancellazione IS NULL
	AND C.IdStato = @STATO_PROVISIONED -- Lo stato del Cliente deve

	IF @@ROWCOUNT <> 1
	BEGIN
		SET @pEsitoConferma = -1
		RAISERROR(N'Errore durante la conferma della richiesta', 16,0)
		RETURN -1;
	END

	SET @pEsitoConferma  = 1; -- OK!
	SELECT @pIdRefereer = Refereer,
		   @pIdCliente = IdCliente
		FROM @tblRichiesta
	
	-- Se la richiesta era associata ad un Cliente, cambiamo lo stato del Cliente a CONFERMATO
	IF @pIdCliente IS NOT NULL
	BEGIN
		UPDATE [Clienti] 
		SET IdStato = @STATO_CONFERMATO
		WHERE Id = @pIdCliente
	END

	COMMIT
	RETURN 0;
END