/*
Tabella che tiene traccia delle associazioni tra gli utenti registrati ed i clienti
*/
CREATE TABLE [dbo].[ClientiUtenti]
(
	[IdCliente]			INT					NOT NULL,
	[IdUtente]			UNIQUEIDENTIFIER	NOT NULL,
	--[TipoAssociazione]	TINYINT				NOT NULL CONSTRAINT DEF_ClientiUtenti_TipoAss DEFAULT (1),,
	[DataCreazione]		DATETIME2(2)		NOT NULL CONSTRAINT DEF_ClientiUtenti_DtCreaz DEFAULT (SYSDATETIME()),
	CONSTRAINT PK_ClientiUtenti PRIMARY KEY(IdCliente, IdUtente),
	CONSTRAINT FK_ClientiUtenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti](Id),
	--CONSTRAINT CHK_ClientiUtenti_TipoAss CHECK (TipoAssociazione IN (1))
)
