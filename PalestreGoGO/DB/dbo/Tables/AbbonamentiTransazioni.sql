CREATE TABLE [dbo].[AbbonamentiTransazioni]
(
	[Id]				INT				NOT NULL IDENTITY(1,1),
	[IdAbbonamento]		INT				NOT NULL,
	[DataTransazione]	DATETIME2		NOT NULL CONSTRAINT DEF_AppuntamentiTransazioni_DtTrans DEFAULT(sysdatetime()),
	[Testo]				VARCHAR(1000)	NOT NULL

	CONSTRAINT PK_AppuntamentiTransazioni PRIMARY KEY (Id)
)
