CREATE PROCEDURE [dbo].[Clienti_Add]
	@pNome				NVARCHAR(100),
	@pRagioneSociale	NVARCHAR(100),
	@pEmail				NVARCHAR(100),
	@pNumTelefono		VARCHAR(50) = NULL,
	@pDescrizione		NVARCHAR(100) = NULL,
	@pIdTipologia		INT,
	@pIndirizzo			NVARCHAR(250),
	@pCitta				NVARCHAR(100),
	@pZipCode			NVARCHAR(10) = NULL,
	@pCountry			NVARCHAR(100) = NULL,
	@pLatitudine		FLOAT = NULL,
	@pLongitudione		FLOAT = NULL,
	@pUrlRoute			VARCHAR(200),
	@pOrarioApertura	VARCHAR(MAX),
	@pStorageContainer	NVARCHAR(500),
	@pIdUserOwner		VARCHAR(100) = NULL,
	@pId				INT OUTPUT,
	@pSecurityToken		NVARCHAR(500) OUTPUT
AS
BEGIN
	SET @pSecurityToken = CAST(NEWID() AS VARCHAR(100));

	INSERT INTO Clienti(Nome, RagioneSociale, Email, NumTelefono, Descrizione, IdTipologia, Indirizzo, Citta, ZipOrPostalCode, Country, Latitudine, Longitudine, 
					   IdUserOwner, SecurityToken, UrlRoute, OrarioApertura, StorageContainer) 
		VALUES(@pNome, @pRagioneSociale, @pEmail, @pNumTelefono, @pDescrizione, @pIdTipologia, @pIndirizzo, @pCitta, @pZipCode, @pCountry, @pLatitudine, @pLongitudione,
						@pIdUserOwner, @pSecurityToken, @pUrlRoute, @pOrarioApertura, @pStorageContainer)
	SET @pId = SCOPE_IDENTITY();
END