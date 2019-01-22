CREATE PROCEDURE [dbo].[ListeAttesa_List4User]
	@pIdCliente			INT = NULL, --Se non valorizzato, cross cliente (profilo utente)
	@pUserId			VARCHAR(100),
	@pIncludeConverted	BIT = 0,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT  @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0),
			@pIncludeConverted = COALESCE(@pIncludeConverted, 0);


	SELECT	l.*,
			s.*	
	FROM vListeAttesa l
		INNER JOIN vSchedules s ON l.IdScheduleListeAttesa = s.IdSchedules
	WHERE @pUserId = @pUserId
	AND ((@pIdCliente IS NULL) OR (l.IdClienteListeAttesa = @pIdCliente))
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (l.DataCancellazioneListeAttesa IS NULL))
	AND ((COALESCE(@pIncludeConverted, 0) = 1) OR (l.DataConversioneListeAttesa IS NULL))
END