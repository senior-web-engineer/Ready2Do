/*
In questa tabella finiscono tutte le informazioni aggiuntive su un cliente come:
Tags ==> Key = Tags, Value = ["aaa", "bbb", ..., "xxx"]
Orari Apertura ==> Key = Orari, Value = JSON Object da definire in base alla UI
*/
CREATE TABLE [dbo].[ClientiMetadati]
(
	[IdCliente]		INT				NOT NULL,
	[Key]			NVARCHAR(100)	NOT NULL,
	[Value]			NVARCHAR(MAX)	NOT NULL,

	CONSTRAINT PK_ClientiTags PRIMARY KEY  ([IdCliente], [Key]),
	CONSTRAINT FK_ClientiTags_Clienti FOREIGN KEY (IdCliente) REFERENCES dbo.Clienti([Id])
)
