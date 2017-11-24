CREATE TABLE [dbo].[Locations]
(
	[Id]			INT				NOT NULL IDENTITY(1,1),
	[IdCliente]		INT				NOT NULL,
	[Nome]			NVARCHAR(100)	NOT NULL,
	[Descrizione]	NVARCHAR(100)	NOT NULL,
	[CapienzaMax]	SMALLINT		NULL,
	
	CONSTRAINT PK_Locations PRIMARY KEY (Id),
	CONSTRAINT FK_Locations_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	--INDEX IDX_UQ_IdIdCliente UNIQUE (Id, IdCliente),
)
