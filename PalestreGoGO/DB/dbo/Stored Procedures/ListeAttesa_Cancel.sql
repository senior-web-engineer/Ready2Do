/*
Annulla l'iscrizione di un utente ad una lista di attesa
*/
CREATE PROCEDURE [dbo].[ListeAttesa_Cancel]
	@pIdCliente		INT, 
	@pUserId		VARCHAR(100),
	@pId			INT,	
	@pEsito			INT
AS
BEGIN
	UPDATE ListeAttesa
		SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	AND UserId = @pUserId
	AND	DataCancellazione IS NULL -- Non deve già essere cancellata
	AND DataConversione IS NULL -- Se convertita in appuntamento non è cancellabile
END