CREATE PROCEDURE [dbo].[Clienti_Add_Undo]
	@pIdCliente			int,
	@pCorrelationId		uniqueidentifier
AS
BEGIN
	DECLARE @STATO_PROVISIONED TINYINT = 3, --Valore fisso corrispondente al record in StatiCliente,
			@STATO_NOTPROVISIONED TINYINT = 1 --Valore fisso corrispondente al record in StatiCliente
	BEGIN TRANSACTION

	DELETE ClientiImmagini WHERE IdCliente = @pIdCliente

	DELETE Clienti
		WHERE Id = @pIdCliente 
		AND CorrelationId = @pCorrelationId
		AND IdStato <> @STATO_PROVISIONED
		AND DataProvisioning IS NULL

	IF @@ROWCOUNT = 1
	BEGIN
		COMMIT
	END
	ELSE
	BEGIN
		ROLLBACK
		RAISERROR('Impossibile annullare l''operazione, il Cliente non è in uno stato annullabile', 16, 0);
		RETURN -1
	END
	
END