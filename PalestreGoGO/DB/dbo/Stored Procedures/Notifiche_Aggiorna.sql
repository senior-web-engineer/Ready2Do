CREATE PROCEDURE [dbo].[Notifiche_Aggiorna]
	@pUserId		VARCHAR(100),
	@pIdNotifica	bigint,
	@pDataView		datetime2(2) = NULL,
	@pDataDismiss	datetime2(2) = NULL
AS
BEGIN
	UPDATE [Notifiche]
		SET DataPrimaVisualizzazione = COALESCE(DataPrimaVisualizzazione , @pDataView),
			DataDismissione = COALESCE(DataDismissione, @pDataDismiss)
	WHERE Id = @pIdNotifica
END