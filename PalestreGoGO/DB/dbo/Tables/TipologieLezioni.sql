CREATE TABLE [dbo].[TipologieLezioni]
(
	[Id]							INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]						INT				NOT NULL,
	[Nome]							NVARCHAR(100)	NOT NULL,
	[Descrizione]					NVARCHAR(500)	NULL,
	[Durata]						INT				NOT NULL,
	[MaxPartecipanti]				INT				NULL,
	[LimiteCancellazioneMinuti]		SMALLINT		NULL, -- Numero di minuti PRIMA dell'inizio dell'evento oltre i quali non è possibile cancellare un'iscrizione
	[Livello]						SMALLINT		NOT NULL CONSTRAINT DEF_TipologieLezioniLivello DEFAULT (0),
	[Prezzo]						DECIMAL(10,2)	NULL, -- Eventuale prezzo della lezione se non inclusa in un abbonamento o per i non abbonati
	[DataCancellazione]				DATETIME2(2)	NULL,
	[DataCreazione]					DATETIME2(2)	NOT NULL CONSTRAINT DEF_TipologieLezioniDataCreaz DEFAULT (SYSDATETIME()),

	CONSTRAINT PK_TipologieLezioni PRIMARY KEY ([Id]),
	CONSTRAINT FK_TipologieLezioni_Clienti FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Clienti]([Id]),

	--20181109-Aggiunta univocità Nome per cliente
	CONSTRAINT UQ_TipologieLezioni_ClienteNome UNIQUE(IdCliente, Nome)
)
