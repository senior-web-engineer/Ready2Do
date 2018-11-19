CREATE PROCEDURE [dbo].[Clienti_Utenti_CertificatoSave]
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pDataPresentazione	datetime2(2),
	@pDataScadenza		datetime2(2),
	@pNote				nvarchar(max),
	@pIdCertificato		int = null OUTPUT	--Se valorizzato in INPUT verrà fatto un update
AS
BEGIN	

IF @pIdCertificato IS NULL
BEGIN
	INSERT INTO ClientiUtentiCertificati(IdCliente, UserId, DataPresentazione, DataScadenza, Note)
		VALUES (@pIdCliente, @pUserId, @pDataPresentazione, @pDataScadenza, @pNote)
	SET @pIdCertificato = SCOPE_IDENTITY();
END
ELSE
BEGIN
	UPDATE ClientiUtentiCertificati
		SET DataPresentazione = @pDataPresentazione,
			DataScadenza = @pDataScadenza,
			Note = @pNote
	WHERE Id = @pIdCertificato
	AND DataCancellazione IS NULL; --Una volta cancellato non è più modificabile

	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR (N'Parametri non validi', 16, 0);
		RETURN -1
	END
END

RETURN 0;
END
