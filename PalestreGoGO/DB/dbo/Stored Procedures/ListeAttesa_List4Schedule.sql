CREATE PROCEDURE [dbo].[ListeAttesa_List4Schedule]
	@pIdCliente			INT,
	@pIdSchedule		INT,
	@pUserId			VARCHAR(100) = NULL, --Se valorizzato indica che si vuole solo la registrazione dello specifico utente
	@pIncludeConverted	BIT = 0,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT  @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0),
			@pIncludeConverted = COALESCE(@pIncludeConverted, 0);


	SELECT	l.*,
			cu.*
	FROM vListeAttesa l
		INNER JOIN vClientiUtenti cu ON cu.IdClienteClientiUtenti = l.IdClienteListeAttesa AND cu.UserIdClientiUtenti = l.UserIdListeAttesa
	WHERE l.IdClienteListeAttesa = @pIdCliente
	AND l.IdScheduleListeAttesa = @pIdSchedule
	AND ((@pUserId IS NULL) OR (l.UserIdListeAttesa = @pUserId)) 
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (l.DataCancellazioneListeAttesa IS NULL))
	AND ((COALESCE(@pIncludeConverted, 0) = 1) OR (l.DataConversioneListeAttesa IS NULL))
END