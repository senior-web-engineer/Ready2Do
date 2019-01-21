CREATE PROCEDURE [dbo].[ListeAttesa_List4User]
	@pIdCliente			INT = NULL, --Se non valorizzato, cross cliente (profilo utente)
	@pUserId			VARCHAR(100),
	@pIncludeConverted	BIT = 0,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT  @pIncludeDeleted = COALESCE(@pIncludeDeleted, 0),
			@pIncludeConverted = COALESCE(@pIncludeConverted, 0);


	SELECT	l.Id,
			l.IdSchedule,
			l.IdCliente,
			l.IdAbbonamento,
			l.DataCreazione,
			l.DataScadenza,
			l.DataConversione,
			l.DataCancellazione,
			l.CausaleCancellazione,
			cu.UserId,
			cu.Nome,
			cu.Cognome,
			cu.UserDisplayName,
			cu.DataCreazione AS DataCreazioneUtente,
			cu.DataCancellazione AS DataCancellazioneUtete,
			cu.DataAggiornamento			
	FROM ListeAttesa l
		INNER JOIN vSchedules
		INNER JOIN ClientiUtenti cu ON cu.IdCliente = l.IdCliente AND cu.UserId = l.UserId		
	WHERE @pUserId = @pUserId
	AND ((@pIdCliente IS NULL) OR (l.IdCliente = @pIdCliente))
	AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (l.DataCancellazione IS NULL))
	AND ((COALESCE(@pIncludeConverted, 0) = 1) OR (l.DataConversione IS NULL))
END