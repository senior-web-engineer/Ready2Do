CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Rifiuta]
	@pIdCliente					int,
	@pIdSchedule				int,
	@pIdAppuntamentoDaConf		int,
	@pMotivo					nvarchar(MAX) = NULL
AS
BEGIN
	DECLARE @MAX_LIV_CORSI SMALLINT = 32767;

	DECLARE @dataOp				DATETIME2 = SYSDATETIME(),
			@userId				VARCHAR(100)

	DECLARE @tblOut			TABLE(UserId VARCHAR(100))
		UPDATE AppuntamentiDaConfermare
			SET DataEsito = @dataOp,
				MotivoRifiuto = @pMotivo
			OUTPUT inserted.UserId INTO @tblOut(UserId)
		WHERE Id = @pIdAppuntamentoDaConf
		AND IdCliente = @pIdCliente
		AND ScheduleId = @pIdSchedule
		AND DataEsito IS NULL
		AND DataExpiration > @dataOp

		IF @@ROWCOUNT <> 1
		BEGIN
			RAISERROR('Impossibile annullare l''appuntamento. Appuntamento non trovato o in stato non valido', 16, 0);
			RETURN -1			
		END

		SELECT @userId = UserId FROM @tblOut;

		-- Generiamo l'evento di notifica
		EXEC [dbo].[internal_AppuntamentoDaConfermare_NotifyRifiuto] @pIdCliente, @pIdAppuntamentoDaConf, @pIdSchedule, @userId
	COMMIT
END