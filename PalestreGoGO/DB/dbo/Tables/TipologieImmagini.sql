CREATE TABLE [dbo].[TipologieImmagini]
(
		[Id]			INT				NOT NULL IDENTITY(1,1),
		[Codice]		VARCHAR(5)		NOT NULL,
		[Nome]			NVARCHAR(100)	NOT NULL,
		[Descrizione]	NVARCHAR(500)	NOT NULL,
		[DataCreazione]	DATETIME2(2)	NOT NULL CONSTRAINT DEF_TipoImmaggini_DataCreaz DEFAULT (SYSDATETIME()),

		CONSTRAINT PK_TipologieImmagini PRIMARY KEY (Id),
		CONSTRAINT UQ_TipologieImmagini_Codice UNIQUE ([Codice]),
)
