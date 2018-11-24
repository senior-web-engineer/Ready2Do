CREATE VIEW [dbo].[vTipologieNotifiche]
AS 
SELECT	Id					AS IdTipologieNotifiche
		,Code				AS CodeTipologieNotifiche
		,UserDismissable	AS UserDismissableTipologieNotifiche
		,AutoDismissAfter	AS AutoDismissAfterTipologieNotifiche
		,[Priority]			AS PriorityTipologieNotifiche
FROM TipologieNotifiche