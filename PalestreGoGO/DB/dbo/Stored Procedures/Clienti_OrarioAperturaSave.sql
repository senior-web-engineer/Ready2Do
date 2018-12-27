CREATE PROCEDURE [dbo].[Clienti_OrarioAperturaSave]
	@pIdCliente			INT,
	@pOrarioApertura	VARCHAR(MAX)
AS
BEGIN
	UPDATE  Clienti
		SET OrarioApertura = @pOrarioApertura
	WHERE Id = @pIdCliente
	AND IdStato >=3 --Solo PROVISIONED

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare il Cliente [%i] da aggiornare',16,1,@pIdCliente);
		RETURN -1
	END
END