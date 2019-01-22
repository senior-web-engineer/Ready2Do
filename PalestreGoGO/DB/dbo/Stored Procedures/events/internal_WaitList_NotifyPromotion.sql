CREATE PROCEDURE [dbo].[internal_WaitList_NotifyPromotion]
	@pIdCliente			int,
	@pIdWaitList		int,
	@pIdSchedule		int,
	@pIdAppuntamento	int,
	@pUserId			VARCHAR(100),
	@pIdAbbonamento		int
AS
BEGIN
	DECLARE @json NVARCHAR(MAX)
	SELECT @json = (
		select 'WaitingList.Promotion' AS EventType,
					@pIdWaitList AS IdWaitList,
					@pIdCliente AS IdCliente,
					@pUserId AS UserId,
					@pIdSchedule AS IdSchedule,
					@pIdAppuntamento AS IdAppuntamento,
					@pIdAbbonamento AS IdAbbonamento,
					SYSDATETIME() AS ChangeDate
		FOR JSON PATH
		)

	INSERT INTO [SystemEvents] ([EventType], [EventSubType], [EventPayload])
		SELECT 'WaitingList.Promotion', NULL, @json

	RETURN 1;
END