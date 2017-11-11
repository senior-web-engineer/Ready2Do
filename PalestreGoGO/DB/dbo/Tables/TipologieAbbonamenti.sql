CREATE TABLE [dbo].[TipologieAbbonamenti]
(
	[Id]			INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]		INT				NOT NULL,
	[Nome]			NVARCHAR(100)	NOT NULL,
	[DurataMesi]	SMALLINT		NULL,
	[NumIngressi]	SMALLINT		NULL,
	[Costo]			DECIMAL(10,2)	NULL,
	[MaxLivCorsi]	SMALLINT		NULL,


	CONSTRAINT PK_TipologieAbbonamenti PRIMARY KEY ([Id]),
	-- Durata e Num Ingressi potrebbero esistere contemporaneamente (num ingressi entro x mesi)
	--CONSTRAINT CK_TipologieAbbonamenti CHECK (([DurataMesi] IS NOT NULL) OR ([NumIngressi] IS NOT NULL)),
	CONSTRAINT FK_TipologieAbbonamenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti](Id),
	--INDEX IDX_UQ_TipologieAbbonamenti_IdIdCliente UNIQUE (Id, IdCliente)
)
