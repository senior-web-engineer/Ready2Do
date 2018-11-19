CREATE PROCEDURE [dbo].[Clienti_Utenti_CertificatoDelete]
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pIdCertificato		int 
AS
BEGIN
	UPDATE ClientiUtentiCertificati
		SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pIdCertificato
	AND IdCliente = @pIdCliente
	AND UserId = @pUserId
	AND DataCancellazione IS NULL;

	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR (N'Parametri non validi', 16, 0);
		RETURN -1
	END

END