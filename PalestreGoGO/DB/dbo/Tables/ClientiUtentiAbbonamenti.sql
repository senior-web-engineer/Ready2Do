/*
Associativa tra la coppia Utente-Cliente (l'utente associato ad uno specifico cliente) ed uno o più abbonamenti 
NOTA: Al momento non viene utilizzata! Ha lo scopo di gestire lo storico degli abbonamenti per un utente
*/
CREATE TABLE [dbo].[ClientiUtentiAbbonamenti]
(
	[IdCliente]					INT					NOT NULL,
	[UserId]					VARCHAR(100)		NOT NULL,
	[IdAbbonamentiUtente]		INT					NOT NULL,

	CONSTRAINT PK_ClientiUtentiAbbonamenti PRIMARY KEY ([IdCliente], [UserId], [IdAbbonamentiUtente]),
	CONSTRAINT FK_ClientiUtentiAbbonamenti_ClientiUtenti FOREIGN KEY (IdCliente, UserId) REFERENCES [ClientiUtenti](IdCliente, UserId),
	CONSTRAINT FK_ClientiUtentiAbbonamenti_AbbonamentiUtenti FOREIGN KEY ([IdAbbonamentiUtente]) REFERENCES [AbbonamentiUtenti]([Id])
)

