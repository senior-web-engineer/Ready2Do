CREATE TABLE [dbo].[RichiesteRegistrazione]
(
	Id					INT					NOT NULL	IDENTITY(1,1),
	DataRichiesta		DATETIME2(2)		NOT NULL	CONSTRAINT DEF_DataRichiestaRegistrazione DEFAULT SYSDATETIME(),
	CorrelationId		UNIQUEIDENTIFIER	NOT NULL,
	UserCode			VARCHAR(1000)		NOT NULL,
	Username			VARCHAR(500)		NOT NULL,
	Expiration			DATETIME2(2)		NOT NULL,
	DataConferma		DATETIME2(2)		NULL,

	CONSTRAINT PK_RichiesteRegistrazioni PRIMARY KEY (Id)
)
