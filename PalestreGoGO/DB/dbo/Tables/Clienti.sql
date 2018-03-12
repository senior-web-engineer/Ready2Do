CREATE TABLE [dbo].[Clienti]
(
	[Id]				INT					NOT NULL IDENTITY(1,1),
	[Nome]				NVARCHAR(100)		NOT NULL,
	[RagioneSociale]	NVARCHAR(100)		NOT NULL,
	[Email]				VARCHAR(100)		NOT NULL,
	[NumTelefono]		VARCHAR(50)			NULL,
	[Descrizione]		NVARCHAR(1000)		NULL,
	[IdTipologia]		INT					NOT NULL, 
	[Indirizzo]			NVARCHAR(250)		NOT NULL,
	[Citta]				NVARCHAR(100)		NOT NULL,
	[ZipOrPostalCode]	NVARCHAR(10)		NULL,
	[Country]			NVARCHAR(100)		NULL,
	[Latitudine]		FLOAT				NULL,
	[Longitudine]		FLOAT				NULL,
	[DataCreazione]		DATETIME2(2)		NOT NULL CONSTRAINT DEF_Clienti_DataCreaz DEFAULT(SYSDATETIME()),
	[IdUserOwner]		UNIQUEIDENTIFIER	NULL, --Utente che ha fatto la registrazione del Cliente (valorizzato solo al provisioning)
	-- Valore casuale utilizzato come meccanismo di sicurezza per la validazione
	[SecurityToken]		NVARCHAR(500)		NOT NULL CONSTRAINT DEF_Clienti_ProvToken DEFAULT(CAST(NEWID() AS NVARCHAR(100))),
	-- Valorizzata solo dopo la conferma dell'utente che l'ha creata e dopo che il sistema ha eseguito il provisioning
	[DataProvisioning]	DATETIME2(2)		NULL,
	[UrlRoute]			VARCHAR(205)		NOT NULL, --URL Route del cliente (eventualmente subdomain)
	[OrarioApertura]	NVARCHAR(MAX)		NULL,
	[StorageContainer]	NVARCHAR(500)		NOT NULL CONSTRAINT DEF_Clienti_ContainerName DEFAULT(LOWER(CAST(NEWID() AS NVARCHAR(100)))),
	
	CONSTRAINT PK_Clienti PRIMARY KEY ([Id]),
	CONSTRAINT FK_Clienti_Tipologia FOREIGN KEY (IdTipologia) REFERENCES [dbo].[TipologieClienti](Id),
	CONSTRAINT [UQ_Clienti_Email] UNIQUE ([Email]),
	CONSTRAINT [UQ_Clienti_UrlRoute] UNIQUE ([UrlRoute]),
	CONSTRAINT [UQ_Clienti_SecurityToken] UNIQUE ([SecurityToken]),
	CONSTRAINT [UQ_Clienti_StorageContainer] UNIQUE ([StorageContainer])
)