CREATE TABLE [dbo].[TipologieAbbonamenti]
(
	[Id]				INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]			INT				NOT NULL,
	[Nome]				NVARCHAR(100)	NOT NULL,
	[DurataMesi]		SMALLINT		NULL,
	[NumIngressi]		SMALLINT		NULL,
	[Costo]				DECIMAL(10,2)	NULL,
	[MaxLivCorsi]		SMALLINT		NULL,
	[ValidoDal]			DATETIME2(2)	NOT NULL CONSTRAINT [DEF_TipologieAbbonamenti_ValidoDal] DEFAULT (sysdatetime()),
	[ValidoFinoAl]		DATETIME2(2)	NULL,
	[DataCreazione]		DATETIME2(2)	NOT NULL CONSTRAINT DEF_TipologieAbbonamenti_DataCreazione default(sysdatetime()),
	[DataCancellazione]	DATETIME2(2)	NULL,

	CONSTRAINT PK_TipologieAbbonamenti PRIMARY KEY ([Id]),
	-- ATTENZIONE: Almeno uno tra duratamesi e numIngressi dovrebbe essere valorizzato
	-- Durata e Num Ingressi potrebbero esistere contemporaneamente (num ingressi entro x mesi)
	CONSTRAINT CK_TipologieAbbonamenti CHECK (([DurataMesi] IS NOT NULL) OR ([NumIngressi] IS NOT NULL)),
	CONSTRAINT FK_TipologieAbbonamenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	INDEX IDXUQ_TipologieAbbonamenti_Nome UNIQUE (IdCliente, Nome) WHERE DataCancellazione IS NULL
)
