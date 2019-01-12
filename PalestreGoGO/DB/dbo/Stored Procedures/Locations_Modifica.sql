CREATE PROCEDURE [dbo].[Locations_Modifica]
	@pIdCliente		INT,
	@pIdLocation	INT,
	@pNome			NVARCHAR(100),
	@pDescrizione	NVARCHAR(MAX),
	@pCapienzaMax	SMALLINT,
	@pColore		VARCHAR(10) = NULL,
	@pImageUrl		VARCHAR(1000) = NULL,
	@pIconUrl		VARCHAR(1000) = NULL,
	@pId			INT OUT
AS
BEGIN
	
	UPDATE Locations
		SET Nome = @pNome,
			Descrizione = @pDescrizione,
			CapienzaMax = @pCapienzaMax,
			Colore = @pColore,
			ImageUrl = @pImageUrl,
			IconUrl = @pIconUrl
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	AND DataCancellazione IS NULL

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare la Location [%i] da aggiornare per il Cliente [%i]' ,16, 0, @pIdLocation, @pIdCliente);
		RETURN -1;
	END
END