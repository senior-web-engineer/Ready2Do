CREATE PROCEDURE [dbo].[TipologieAbbonamenti_Add]
	@pIdCliente						INT,
	@pNome							NVARCHAR(100),
	@pDurataMesi					SMALLINT = NULL,
	@pNumIngressi					SMALLINT = NULL,
	@pCosto							DECIMAL(10,2) = NULL,
	@pMaxLivCorsi					SMALLINT = NULL,
	@pValidoDal						DATETIME2(2),
	@pValidoFinoAl					DATETIME2(2) = NULL,
	@pId							INT OUTPUT
AS
BEGIN

	INSERT INTO TipologieAbbonamenti(IdCliente, Nome, DurataMesi, NumIngressi, Costo, MaxLivCorsi, ValidoDal, ValidoFinoAl)
		VALUES (@pIdCliente, @pNome, @pDurataMesi, @pNumIngressi, @pCosto, @pMaxLivCorsi, @pValidoDal, @pValidoFinoAl)

	SET @pId = SCOPE_IDENTITY()
	
	RETURN 0
END
