/*
Cancella logicamente una richiesta di cancellazione SOLO se non è già stata confermata
*/
CREATE PROCEDURE [dbo].[RichiestaRegistrazione_GetLast]
	@pUsername			VARCHAR(500)
AS
BEGIN
	
	SELECT TOP 1 *
	FROM [RichiesteRegistrazione] rr
	WHERE Username = @pUsername
	ORDER BY DataRichiesta DESC


	RETURN 0;
END