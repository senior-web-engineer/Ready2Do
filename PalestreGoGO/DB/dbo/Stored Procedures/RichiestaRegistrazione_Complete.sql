CREATE PROCEDURE [dbo].[RichiestaRegistrazione_Complete]
	@pUserCode			VARCHAR(1000),
	@pUsername			VARCHAR(500)
AS
BEGIN
	DECLARE @dtOp DATETIME2 = SYSDATETIME()
	
	UPDATE [RichiesteRegistrazione]
		SET DataConferma = @dtOp
	OUTPUT inserted.Id, inserted.CorrelationId, inserted.DataConferma, inserted.DataRichiesta, inserted.Expiration, inserted.UserCode, inserted.Username
	WHERE Username = @pUsername
	AND UserCode = @pUserCode
	AND Expiration > @dtOp
	AND DataConferma IS NULL 

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Errore durante la conferma della richiesta', 16,0)
		RETURN -1;
	END

	RETURN 0;
END