CREATE TABLE [dbo].[ClientiImmagini]
(
	[Id]				INT				NOT NULL IDENTITY(1,1),
	[IdCliente]			INT				NOT NULL,
	[IdTipoImmagine]	INT				NOT NULL,
	[Nome]				NVARCHAR(100)	NOT NULL,
	[Alt]				NVARCHAR(100)	NULL,
	[Url]				NVARCHAR(1000)	NOT NULL,
	[ThumbnailUrl]		NVARCHAR(1000)	NULL,
	[Descrizione]		NVARCHAR(1000)	NULL,
	[Ordinamento]		INT				NOT NULL CONSTRAINT DEF_ClientiImg_Order DEFAULT(0),
	[DataCancellazione]	DATETIME2(2)	NULL,

	CONSTRAINT PK_ClientiImmagini PRIMARY KEY (Id),
	CONSTRAINT FK_ClientiImmagini_Clienti FOREIGN KEY (IdCliente) REFERENCES [dbo].[Clienti]([Id]),
	CONSTRAINT FK_ClientiImmagini_TipologiaImmagini FOREIGN KEY (IdTipoImmagine) REFERENCES [dbo].[TipologieImmagini]([Id])
)
