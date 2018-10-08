CREATE PROCEDURE [dbo].[RichiestaRegistrazione_Insert]
	@pUserCode			VARCHAR(1000),
	@pUsername			VARCHAR(500),
	@pCorrelationId		UNIQUEIDENTIFIER = NULL,
	@pExpiration		DATETIME2(2) = NULL
AS
BEGIN
	DECLARE @id INT;
	SELECT @pCorrelationId = COALESCE(@pCorrelationId, NEWID()),
		   @pExpiration = COALESCE(@pExpiration, DATEADD(DAY, 1, SYSDATETIME()))

	INSERT INTO RichiesteRegistrazione(CorrelationId, UserCode, Username, Expiration)
		VALUES (@pCorrelationId, @pUserCode, @pUsername, @pExpiration)

	SET @id =SCOPE_IDENTITY()

	SELECT r.Id,
		   r.CorrelationId,
		   r.DataConferma,
		   r.DataRichiesta,
		   r.Expiration,
		   r.UserCode,
		   r.Username
	FROM RichiesteRegistrazione r
	WHERE r.Id = @id
END
