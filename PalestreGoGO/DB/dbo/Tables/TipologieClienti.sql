CREATE TABLE [dbo].[TipologieClienti]
(
	[Id]					INT				NOT NULL,
	[Nome]					NVARCHAR(50)	NOT NULL,
	[Descrizione]			NVARCHAR(100)	NULL,
	[DataCreazione]			DATETIME2(2)	NOT NULL CONSTRAINT DEF_TipologieCliente_Creaz default(sysdatetime()),
	[DataCancellazione]		DATETIME2(2)	NULL,

	CONSTRAINT PK_TipologieClienti PRIMARY KEY([Id])
)
