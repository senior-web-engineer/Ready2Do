/*
Certificati presentati da un Utente ad uno specifico cliente
Se un utente non ha ancora presentato un certificato, non comparirà in questa tabella
*/
CREATE TABLE [dbo].[ClientiUtentiCertificati]
(
	[Id]					INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]				INT				NOT NULL,
	[UserId]				VARCHAR(100)	NOT NULL,
	[DataPresentazione]		DATETIME2(2)	NOT NULL,
	[DataScadenza]			DATETIME2(2)	NOT NULL,
	[Note]					NVARCHAR(MAX)	NULL,
	[DataCancellazione]		DATETIME2(2)	NULL,

	CONSTRAINT PK_ClientiUtentiCertificati PRIMARY KEY (Id),
	CONSTRAINT FK_ClientiUtentiCertificati_ClientiUtenti FOREIGN KEY(IdCliente, UserId) REFERENCES [ClientiUtenti]([IdCliente], UserId),
)
