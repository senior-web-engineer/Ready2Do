CREATE TABLE [dbo].[AbbonamentiTransazioni]
(
	[Id]				INT				NOT NULL IDENTITY(1,1),
	[IdAbbonamento]		INT				NOT NULL,
	[TipoTransazione]	CHAR(3)			NOT NULL CONSTRAINT CHK_AbbonamentiTransazioni_Tipo CHECK([TipoTransazione] IN  ('APP', 'WLI', 'WLD', 'CAP')),
	[DataTransazione]	DATETIME2		NOT NULL CONSTRAINT DEF_AbbonamentiTransazioni_DtTrans DEFAULT(sysdatetime()),
	[Quantita]			INT				NOT NULL,
	[Payload]			NVARCHAR(500)	NULL CONSTRAINT CHK_AbbonamentiTransazioni_PyaloadJson CHECK(ISJSON([Payload]) = 1)

	CONSTRAINT PK_AppuntamentiTransazioni PRIMARY KEY (Id),
	CONSTRAINT FK_AbbonamentiTransazioni_AbbonamentiUtenti FOREIGN KEY (IdAbbonamento) REFERENCES AbbonamentiUtenti(Id)
)
