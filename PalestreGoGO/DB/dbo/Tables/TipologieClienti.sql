CREATE TABLE [dbo].[TipologieClienti]
(
	[Id]					SMALLINT		NOT NULL,
	[Nome]					NVARCHAR(50)	NOT NULL,
	[Descrizione]			NVARCHAR(100)	NULL,

	CONSTRAINT PK_TipologieClienti PRIMARY KEY([Id])
)
