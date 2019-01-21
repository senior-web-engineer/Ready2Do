/*
Tabella che tiene traccia delle associazioni tra gli utenti registrati ed i clienti
ATTENZIONE! Non essendoci una chiave surrogata (Id) c'è un problema nella gestire lo storico,
quando andiamo in Join verso questa tabella, se non esiste più un'associazione valida e ce ne sono più di una storicizzate,
c'è un problema per determinare qual'è quella valida "al tempo"
*/
CREATE TABLE [dbo].[ClientiUtenti]
(
	[IdCliente]			INT					NOT NULL,
	[UserId]			VARCHAR(100)		NOT NULL,
	[Nome]				NVARCHAR(100)		NOT NULL,
	[Cognome]			NVARCHAR(100)		NOT NULL,
	[UserDisplayName]	NVARCHAR(100)		NOT NULL,
	[DataAggiornamento] DATETIME2(2)		NOT NULL CONSTRAINT DEF_ClientiUtenti_DtAggiorn DEFAULT (SYSDATETIME()),
	[DataCreazione]		DATETIME2(2)		NOT NULL CONSTRAINT DEF_ClientiUtenti_DtCreaz DEFAULT (SYSDATETIME()),
	[DataCancellazione] DATETIME2(2)		NULL,
	CONSTRAINT PK_ClientiUtenti PRIMARY KEY(IdCliente, UserId),
	CONSTRAINT FK_ClientiUtenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti](Id),
	INDEX UQX_ClientiUtenti UNIQUE (IdCliente, UserId) WHERE DataCancellazione IS NULL, -- Solo un'associazione attiva
	--Aggiungiamo questo constraint per gestire l'assenza della chiave surrogata, gestiamo solo una versione dello storico
	INDEX UQX_ClientiUtentiStorico UNIQUE (IdCliente, UserId) WHERE DataCancellazione IS NOT NULL -- Solo un'associazione storicizzata
)
GO
