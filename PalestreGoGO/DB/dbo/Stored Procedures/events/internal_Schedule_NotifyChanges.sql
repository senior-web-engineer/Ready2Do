/*
La procedura viene invocata quando viene modificato uno schedule, genera gli eventi corrispondenti alle notifiche
Viene generata un'unica notifica 
*/
CREATE PROCEDURE [dbo].[internal_Schedule_NotifyChanges]
	@pIdCliente			int,
	@pIdSchedule		int,
	@pOldData			datetime2,
	@pNewData			datetime2,
	@pIdOldLocation		int,
	@pIdNewLocation		int
AS
begin
	declare @notifyAppointmnetsOnDateChange			INT,
			@notifyAppointmnetsOnLocationChange		INT,
			@notifyWaitListOnDateChange				INT,
			@notifyWaitListOnLocationChange			INT

	IF (@pOldData<> @pNewData) OR (@pIdOldLocation <> @pIdNewLocation)
	BEGIN
		RETURN 0; -- Niente da notificare
	END

	SELECT @notifyAppointmnetsOnDateChange = [dbo].[ClientePreferenzaGet](@pIdCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_DATAORA.ISCRITTI'),
		   @notifyAppointmnetsOnLocationChange = [dbo].[ClientePreferenzaGet](@pIdCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_LOCATION.ISCRITTI'),
		   @notifyWaitListOnDateChange = [dbo].[ClientePreferenzaGet](@pIdCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_DATAORA.WAITLIST'),
		   @notifyWaitListOnLocationChange = [dbo].[ClientePreferenzaGet](@pIdCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_LOCATION.WAITLIST')

	DECLARE @json NVARCHAR(MAX)
	SELECT @json = (
			select 'Schedule.Changed' AS EventType,
					@pIdschedule AS IdSchedule,
					@pIdCliente AS IdCliente,
					SYSDATETIME() AS ChangeDate,
					(SELECT * FROM (VALUES('NOTIFICHE.SCHEDULE.ONCHANGE_DATAORA.ISCRITTI', @notifyAppointmnetsOnDateChange),
											 ('NOTIFICHE.SCHEDULE.ONCHANGE_LOCATION.ISCRITTI', @notifyAppointmnetsOnLocationChange),
											 ('NOTIFICHE.SCHEDULE.ONCHANGE_DATAORA.WAITLIST', @notifyWaitListOnDateChange),
											 ('NOTIFICHE.SCHEDULE.ONCHANGE_LOCATION.WAITLIST', @notifyWaitListOnLocationChange))
											 AS Pref([Key],[Value]) FOR JSON AUTO) AS [PreferenzeCliente],
					(SELECT * FROM (
						SELECT CASE WHEN @pOldData <> @pNewData THEN 'DataOraInizio' ELSE NULL END AS ChangedFiel, @pOldData as OldValue, @pNewData AS NewValue
						UNION
						SELECT CASE WHEN @pIdOldLocation <> @pIdNewLocation THEN 'IdLocation' ELSE NULL END AS ChangedFiel, 
							 CAST(@pIdOldLocation AS VARCHAR(100)) as OldValue, 
							 CAST(@pIdNewLocation AS VARCHAR(100)) AS NewValue) T
						WHERE T.ChangedFiel IS NOT NULL FOR JSON AUTO) AS [Changes],
					(SELECT * FROM Appuntamenti 
						WHERE ScheduleId = @pIdSchedule 
						AND IdCliente = @pIdCliente
						AND (((@notifyAppointmnetsOnDateChange = 1) AND (@pOldData<>@pNewData)) OR 
						     ((@notifyAppointmnetsOnLocationChange = 1) AND (@pIdOldLocation<>@pIdNewLocation)))
					FOR JSON AUTO) AS [Appuntamenti],
					(SELECT * FROM ListeAttesa
						WHERE IdSchedule = @pIdSchedule 
						AND IdCliente = @pIdCliente
						AND (((@notifyAppointmnetsOnDateChange = 1) AND (@pOldData<>@pNewData)) OR 
						     ((@notifyAppointmnetsOnLocationChange = 1) AND (@pIdOldLocation<>@pIdNewLocation)))
					FOR JSON AUTO) AS [WaitList]
			for json path, root('NotificationEvent') 
			)
	INSERT INTO [SystemEvents] ([EventType], [EventSubType], [EventPayload])
		SELECT 'Schedule.Changed', NULL, @json

	RETURN 1;
END