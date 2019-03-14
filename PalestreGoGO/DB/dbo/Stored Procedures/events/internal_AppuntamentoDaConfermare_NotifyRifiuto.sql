CREATE PROCEDURE [dbo].[internal_AppuntamentoDaConfermare_NotifyRifiuto]
	@pIdCliente							int,
	@pIdAppuntamentoDaConfermare		int,
	@pIdSchedule						int,
	@pUserId							VARCHAR(100)
AS
BEGIN
	DECLARE @json NVARCHAR(MAX)
	SELECT @json = (
		select 'AppuntamentoDaConfermare.Rifiutato' AS EventType,
					@pIdAppuntamentoDaConfermare AS IdAppuntamentoDaConfermare,
					@pIdCliente AS IdCliente,
					@pUserId AS UserId,
					@pIdSchedule AS IdSchedule,
					SYSDATETIME() AS ChangeDate
		FOR JSON PATH
		)

	INSERT INTO [SystemEvents] ([EventType], [EventSubType], [EventPayload])
		SELECT 'AppuntamentoDaConfermare.Rifiutato', NULL, @json

	RETURN 1;
END
