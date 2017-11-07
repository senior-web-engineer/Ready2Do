CREATE TABLE [dbo].[Clienti]
(
	[Id]				INT NOT NULL IDENTITY(1,1),
	[Nome]				NVARCHAR(250),
	[Descrizione]		NVARCHAR(1000),
	[IdTipologia]		SMALLINT, 
	[Indirizzo]			NVARCHAR(250)	NULL,
	[Citta]				NVARCHAR(100)	NOT NULL,
	[ZipOrPostalCode]	NVARCHAR(10)	NOT NULL,
	[Country]			NVARCHAR(100)	NOT NULL,
	[Latitudine]		FLOAT			NULL,
	[Longitudine]		FLOAT			NULL,

	
	CONSTRAINT PK_Clienti PRIMARY KEY (Id),
	CONSTRAINT FK_Clienti_Tipologia FOREIGN KEY (IdTipologia) REFERENCES [dbo].[TipologieClienti](Id),
)
