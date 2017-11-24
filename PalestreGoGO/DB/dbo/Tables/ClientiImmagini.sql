CREATE TABLE [dbo].[ClientiImmagini]
(
	[Id]				INT				NOT NULL IDENTITY(1,1),
	[IdCliente]			INT				NOT NULL,
	[IdTipoImmagine]	INT				NOT NULL,
	[Nome]				NVARCHAR(100)	NOT NULL,
	[Url]				NVARCHAR(1000)	NOT NULL,
	[Descrizione]		NVARCHAR(1000)	NULL,

	CONSTRAINT PK_ClientiImmagini PRIMARY KEY (Id),
	CONSTRAINT FK_ClientiImmagini_Clienti FOREIGN KEY (IdCliente) REFERENCES [dbo].[Clienti]([Id]),
	CONSTRAINT FK_ClientiImmagini_TipologiaImmagini FOREIGN KEY (IdTipoImmagine) REFERENCES [dbo].[TipologieImmagini]([Id])
)
