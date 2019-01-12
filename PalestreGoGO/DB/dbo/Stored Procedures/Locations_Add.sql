CREATE PROCEDURE [dbo].[Locations_Add]
	@pIdCliente		INT,
	@pNome			NVARCHAR(100),
	@pDescrizione	NVARCHAR(MAX),
	@pCapienzaMax	SMALLINT,
	@pColore		VARCHAR(10) = NULL,
	@pImageUrl		VARCHAR(1000) = NULL,
	@pIconUrl		VARCHAR(1000) = NULL,
	@pId			INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM vClientiProvisioned WHERE Id = @pIdCliente)
	BEGIN
		RAISERROR('Impossibile trovare il cliente [%i]', 16, 0, @pIdCliente);
		RETURN -1;
	END

	INSERT INTO Locations(IdCliente, Nome, Descrizione, CapienzaMax, Colore, ImageUrl, IconUrl)
	 VALUES(@pIdCliente, @pNome, @pDescrizione, @pCapienzaMax, @pColore, @pImageUrl, @pIconUrl)

	 SET @pId = SCOPE_IDENTITY();
END