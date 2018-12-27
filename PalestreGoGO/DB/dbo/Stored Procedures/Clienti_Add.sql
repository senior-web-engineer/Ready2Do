/*
Crea un nuovo Cliente non ancora provisioned
Se per il Cliente è già presente un record non provisioned (magari a seguito di una precedente iscrizione non completata o andata in errore)
e sono passati più id @pOldRequestTimeout minuti, la vecchia richiesta viene eliminata e ne viene creata una nuova.
*/
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
	@pStorageContainer	NVARCHAR(500) = NULL,
	@pIdUserOwner		VARCHAR(100) = NULL,
	@pOldRequestTimeout	INT	 = 60, -- Numero di ORE dopo cui una vecchia richiesta può essere sovrascritta (DEFAULT = 1h = 60)
	@pId				INT OUTPUT,
	@pCorrelationId		UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
	DECLARE @userCode	VARCHAR(1000);

	SET @pCorrelationId = COALESCE(@pCorrelationId, NEWID());
	SET @pStorageContainer  = COALESCE(@pStorageContainer, REPLACE(CAST(NEWID() AS VARCHAR(100)), '-', ''))
	--BEGIN TRANSACTION
	--SET XACT_ABORT ON;
	INSERT INTO Clienti(Nome, RagioneSociale, Email, NumTelefono, Descrizione, IdTipologia, Indirizzo, Citta, ZipOrPostalCode, Country, Latitudine, Longitudine, 
					   IdUserOwner, CorrelationId, UrlRoute, StorageContainer) 
		VALUES(@pNome, @pRagioneSociale, @pEmail, @pNumTelefono, @pDescrizione, @pIdTipologia, @pIndirizzo, @pCitta, @pZipCode, @pCountry, @pLatitudine, @pLongitudione,
						@pIdUserOwner, @pCorrelationId, @pUrlRoute, @pStorageContainer)

	SET @pId = SCOPE_IDENTITY();
	
	--EXEC RichiestaRegistrazione_Add @pEmail, @pCorrelationId, @pExpiration = NULL, @pUserCode = @userCode OUTPUT
	--COMMIT
END