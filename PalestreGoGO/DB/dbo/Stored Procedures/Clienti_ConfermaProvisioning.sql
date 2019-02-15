CREATE PROCEDURE [dbo].[Clienti_ConfermaProvisioning]
	@pIdCliente				INT,
	@pAccountConfermato		BIT = 0
AS
BEGIN
	DECLARE @dtOp DATETIME2 = SYSDATETIME(),
			@STATO_PUBLISHED TINYINT = 10,
			@STATO_PROVISIONED TINYINT = 3, --Valore fisso corrispondente al record in StatiCliente,
			@STATO_NOTPROVISIONED TINYINT = 0 --Valore fisso corrispondente al record in StatiCliente
	
	UPDATE Clienti
		SET DataProvisioning = @dtOp,
			-- Lo stato lo impostiamo a seconda che l'utente abbia confermato la propria email o meno
			IdStato = CASE WHEN COALESCE(@pAccountConfermato, 0) = 0 THEN @STATO_PROVISIONED ELSE @STATO_PUBLISHED END
	WHERE Id = @pIdCliente
	AND IdStato = @STATO_NOTPROVISIONED
	AND DataProvisioning IS NULL
	
	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare il Cliente [%i] da confermare', 16, 0, @pIdCliente);
		RETURN -1
	END

	RETURN 0;
END