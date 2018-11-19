CREATE TABLE [dbo].[MailTemplates]
(
	[Id]					INT					NOT NULL	IDENTITY(1,1),
	[TipoMail]				VARCHAR(150)		NOT NULL,	/*1=ConfermaCliente, 2=ConfermaUtente*/
	[Subject]				NVARCHAR(300)		NOT NULL,
	[TextBody]				NVARCHAR(MAX)		NOT NULL,
	[HtmlBody]				NVARCHAR(MAX)		NULL,
	[OnlyText]				BIT					NOT NULL CONSTRAINT MailTemplates_OnlyText DEFAULT(0),
	[DataCancellazione]		DATETIME2(2)		NULL,
	CONSTRAINT PK_MailTemplates PRIMARY KEY (Id),
	INDEX UQ_MailTemplates_Tipo UNIQUE (TipoMail) WHERE DataCancellazione IS NULL
)
GO
