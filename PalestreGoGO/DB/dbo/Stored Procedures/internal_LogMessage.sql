CREATE PROCEDURE [dbo].[internal_LogMessage]
	@pIdCliente			INT,
	@pUserId			VARCHAR(100),
	@pMessageLevel		CHAR(1) = 'I', -- 'E': Error, 'W':Warning, 'I':Info, 'V': Verbose
	@pMessage			NVARCHAR(2000),
	@pDataOperazione	DATETIME2 = NULL
AS
BEGIN
	DECLARE @data NVARCHAR(100)
	SET @pIdCliente = COALESCE(@pIdCliente, -1)
	SET @data = CONVERT(nvarchar(100), COALESCE(@pDataOperazione, SYSDATETIME()), 127)
	SET @pMessageLevel = COALESCE(@pMessageLevel, 'I')
	IF @pMessageLevel NOT IN ('E', 'W', 'I', 'V')
		SET @pMessageLevel = 'L'
	SET @pMessage = CONCAT(@pIdCliente,'||', @pUserId, '||',  @data, '||', @pMessageLevel, '||', @pMessage);
	RAISERROR(@pMessage, 0, 1) WITH NOWAIT
END