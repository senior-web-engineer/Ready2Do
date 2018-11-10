CREATE TABLE [dbo].[ClientiPreferenze]
(
	[IdCliente]			INT				NOT NULL,
	[Key]				VARCHAR(100)	NOT NULL,
	[Value]				VARCHAR(MAX)	NOT NULL,

	CONSTRAINT PK_ClientiPreferenze PRIMARY KEY (IdCliente, [Key]),
	CONSTRAINT FK_ClientiPreferenze_Clienti FOREIGN KEY (IdCliente) REFERENCES Clienti(Id)
)
