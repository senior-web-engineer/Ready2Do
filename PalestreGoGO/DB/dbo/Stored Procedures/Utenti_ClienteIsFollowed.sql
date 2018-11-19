CREATE PROCEDURE [dbo].[Utenti_ClienteIsFollowed]
	@pUserId		varchar(50),
	@pIdCliente		int,
	@pResult		bit OUTPUT
AS
BEGIN
	IF EXISTS(SELECT * FROM ClientiUtenti WHERE IdCliente = @pIdCliente AND UserId = @pUserId)
		SET @pResult = 1;
	ELSE
		SET @pResult = 0
END