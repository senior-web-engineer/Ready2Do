CREATE PROCEDURE [dbo].[Clienti_Utenti_Associa]
	@pIdCliente			int,
	@pUserId			varchar(50),
	@pNome				nvarchar(100),
	@pCognome			nvarchar(100),
	@pDisplayName		nvarchar(100)
AS
BEGIN
	INSERT INTO ClientiUtenti(IdCliente, UserId, Nome, Cognome, UserDisplayName)
		SELECT @pIdCliente, @pUserId, @pNome, @pCognome, @pDisplayName
			WHERE NOT EXISTS(SELECT * FROM ClientiUtenti 
							WHERE IdCliente = @pIdCliente AND UserId = @pUserId
							 AND DataCancellazione IS NULL);
	-- Se non abbiamo inserito niente perché già esisteva, facciamo un update
	IF @@ROWCOUNT = 0
	BEGIN
		UPDATE ClientiUtenti
			SET Nome = @pNome,
				Cognome = @pCognome,
				UserDisplayName = @pDisplayName
		WHERE IdCliente = @pIdCliente
		AND UserId = @pUserId
		AND DataCancellazione IS NULL
	END
END