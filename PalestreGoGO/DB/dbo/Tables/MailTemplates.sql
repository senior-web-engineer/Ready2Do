CREATE TABLE [dbo].[MailTemplates]
(
	[Id]			SMALLINT			NOT NULL	IDENTITY(1,1),
	[TipoMail]		TINYINT				NOT NULL,	/*1=ConfermaCliente, 2=ConfermaUtente*/
	[Subject]		NVARCHAR(300)		NOT NULL,
	[TextBody]		NVARCHAR(MAX)		NOT NULL,
	[HtmlBody]		NVARCHAR(MAX)		NOT NULL

	CONSTRAINT PK_MailTemplates PRIMARY KEY (Id),
)
GO

CREATE UNIQUE INDEX UK_MailTemplates_Tipo ON [MailTemplates](TipoMail)
