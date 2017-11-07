CREATE TABLE [dbo].[TipologieLezioni]
(
	[Id]				INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]			INT				NOT NULL,
	[Nome]				NVARCHAR(100)	NOT NULL,
	[Descrizione]		NVARCHAR(500)	NULL,
	[Durata]			INT				NOT NULL,
	[MaxPartecipanti]	INT				NULL,

	CONSTRAINT PK_TipologieLezioni PRIMARY KEY ([Id]),
	CONSTRAINT FK_TipologieLezioni_Clienti FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Clienti]([Id])
)
