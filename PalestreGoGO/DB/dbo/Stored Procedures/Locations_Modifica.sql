CREATE PROCEDURE [dbo].[Locations_Modifica]
	@pIdCliente		INT,
	@pIdLocation	INT,
	@pNome			NVARCHAR(100),
	@pDescrizione	NVARCHAR(MAX),
	@pCapienzaMax	SMALLINT,
	@pId			INT OUT
AS
BEGIN
	
	UPDATE Locations
		SET Nome = @pNome,
			Descrizione = @pDescrizione,
			CapienzaMax = @pCapienzaMax
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	AND DataCancellazione IS NULL

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare la Location [%i] da aggiornare per il Cliente [%i]' ,16, 0, @pIdLocation, @pIdCliente);
		RETURN -1;
	END
END