CREATE PROCEDURE [dbo].[Appuntamenti_Lista4Schedule]
	@pIdCliente				INT,
	@pIdSchedule			INT,
	@pIncludeDeleted		BIT = 0
AS
BEGIN
	SELECT   a.*,
			cu.Cognome,
			cu.Nome,
			cu.UserDisplayName,
			cu.DataAggiornamento,
			cu.DataCancellazione AS DataCancellazioneUtente,
			cu.DataCreazione AS DataCreazioneUtente
	FROM vAppuntamentiFull a
		LEFT JOIN ClientiUtenti cu ON cu.IdCliente = a.IdCliente AND cu.UserId = a.UserId AND cu.DataCancellazione is null
	WHERE a.IdCliente = @pIdCliente
		AND a.ScheduleId = @pIdSchedule
		AND ((COALESCE(@pIncludeDeleted,0) = 1) OR (a.DataCancellazione IS NULL))
END