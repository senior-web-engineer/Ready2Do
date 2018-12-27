CREATE TABLE [dbo].[Locations]
(
	[Id]					INT				NOT NULL IDENTITY(1,1),
	[IdCliente]				INT				NOT NULL,
	[Nome]					NVARCHAR(100)	NOT NULL,
	[Descrizione]			NVARCHAR(MAX)	NULL,
	[CapienzaMax]			SMALLINT		NULL,
	[DataCreazione]			DATETIME2(2)	NOT NULL CONSTRAINT DEF_Locations_DataCreazione DEFAULT (SYSDATETIME()),
	[DataCancellazione]		DATETIME2(2)	NULL,

	CONSTRAINT PK_Locations PRIMARY KEY (Id),
	CONSTRAINT FK_Locations_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
)
