CREATE PROCEDURE [dbo].[internal_SystemaEvents_Add]
	@pEventType		VARCHAR(500),
	@pEventSubType	VARCHAR(500) = NULL,
	@pPayload		NVARCHAR(MAX)
AS
BEGIN
	INSERT INTO [SystemEvents](EventType, EventSubType, EventPayload)
		VALUES(@pEventType, @pEventSubType, @pPayload);
END