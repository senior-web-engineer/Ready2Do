CREATE PROCEDURE [dbo].[internal_AppuntamentoDaConfermare_NotifyConferma]
	@pIdCliente							int,
	@pIdAppuntamentoDaConfermare		int,
	@pIdSchedule						int,
	@pIdAppuntamento					int,
	@pUserId							VARCHAR(100),
	@pIdAbbonamento						int
AS
BEGIN
	DECLARE @json NVARCHAR(MAX)
	SELECT @json = (
		select 'AppuntamentoDaConfermare.Confermato' AS EventType,
					@pIdAppuntamentoDaConfermare AS IdAppuntamentoDaConfermare,
					@pIdCliente AS IdCliente,
					@pUserId AS UserId,
					@pIdSchedule AS IdSchedule,
					@pIdAppuntamento AS IdAppuntamento,
					@pIdAbbonamento AS IdAbbonamento,
					SYSDATETIME() AS ChangeDate
		FOR JSON PATH
		)

	INSERT INTO [SystemEvents] ([EventType], [EventSubType], [EventPayload])
		SELECT 'AppuntamentoDaConfermare.Confermato', NULL, @json

	RETURN 1;
END
