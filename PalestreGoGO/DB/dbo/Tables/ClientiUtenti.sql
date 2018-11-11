/*
Tabella che tiene traccia delle associazioni tra gli utenti registrati ed i clienti
*/
CREATE TABLE [dbo].[ClientiUtenti]
(
	[IdCliente]			INT					NOT NULL,
	[IdUtente]			VARCHAR(50)			NOT NULL,
	[NominativoUser]	NVARCHAR(250)		NOT NULL,
	[UserDisplayName]	NVARCHAR(100)		NOT NULL,
	[DataAggiornamento] DATETIME2(2)		NOT NULL CONSTRAINT DEF_ClientiUtenti_DtAggiorn DEFAULT (SYSDATETIME()),
	[DataCreazione]		DATETIME2(2)		NOT NULL CONSTRAINT DEF_ClientiUtenti_DtCreaz DEFAULT (SYSDATETIME()),
	[DataCancellazione] DATETIME2(2)		NULL,
	CONSTRAINT PK_ClientiUtenti PRIMARY KEY(IdCliente, IdUtente),
	CONSTRAINT FK_ClientiUtenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti](Id),
	INDEX UQX_ClientiUtenti UNIQUE (IdCliente, IdUtente) WHERE DataCancellazione IS NULL -- Solo un'associazione attiva
)
GO
