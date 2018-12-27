/*
Cancella logicamente una richiesta di cancellazione SOLO se non è già stata confermata
*/
CREATE PROCEDURE [dbo].[RichiestaRegistrazione_Delete]
	@pUsername			VARCHAR(500)
AS
BEGIN
	DECLARE @dtOp DATETIME2 = SYSDATETIME()
	
	UPDATE [RichiesteRegistrazione]
		SET DataCancellazione = @dtOp
	WHERE Username = @pUsername
	AND DataConferma IS NULL 
	AND DataCancellazione IS NULL

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Errore durante la cancellazione della richiesta per l''utente ''%s''. Richiesta non trovata o già confermata', 16,0, @pUsername);
		RETURN -1;
	END

	RETURN 0;
END