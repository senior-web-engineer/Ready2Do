CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_SetExpiration]
	@pIdCliente				int,
	@pIdSchedule			int,
	@pUserId				varchar(100)
AS
BEGIN
	UPDATE AppuntamentiDaConfermare
	SET DataEsito = SYSDATETIME(),
		MotivoRifiuto = 'Scaduto timeout per la conferma dell''appuntamento'
	WHERE IdCliente = @pIdCliente
	AND ScheduleId = @pIdSchedule
	AND UserId = @pUserId
	AND DataCancellazione IS NULL
END