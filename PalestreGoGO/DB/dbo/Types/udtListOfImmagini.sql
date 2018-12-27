CREATE TYPE [dbo].[udtListOfImmagini] AS TABLE
(
	[Id]				INT				NULL,
	[IdCliente]			INT				NOT NULL,
	[IdTipoImmagine]	INT				NOT NULL,
	[Nome]				NVARCHAR(100)	NOT NULL,
	[Alt]				NVARCHAR(100)	NULL,
	[Url]				NVARCHAR(1000)	NOT NULL,
	[ThumbnailUrl]		NVARCHAR(1000)	NULL,
	[Descrizione]		NVARCHAR(1000)	NULL,
	[Ordinamento]		INT				NOT NULL
)
