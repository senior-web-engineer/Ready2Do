CREATE TABLE [dbo].[StatiPagamenti]
(
	[Id]			TINYINT			NOT NULL,
	[Descrizione]	VARCHAR(250)	NOT NULL,

	CONSTRAINT PK_StatiPagamenti PRIMARY KEY (Id),
	CONSTRAINT UQI_StatiPagamenti_Desc UNIQUE (Descrizione)
)
