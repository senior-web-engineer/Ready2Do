CREATE TABLE [dbo].[Schedules]
(
	[Id]						INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]					INT				NOT NULL,
	[Title]						NVARCHAR(100)	NOT NULL,
	[IdTipoLezione]				INT				NOT NULL,
	[IdLocation]				INT				NOT NULL,
	[Data]						DATE			NOT NULL,
	[OraInizio]					TIME			NOT NULL,
	[Istruttore]				NVARCHAR(150)	NULL,
	[PostiDisponibili]			INT				NOT NULL,	-- Posti inizilamente disponibili
	[PostiResidui]				INT				NOT NULL,	-- Contatore dei posti ancora disponibili
	[CancellabileFinoAl]		DATETIME2		NOT NULL,
	[DataChiusuraIscrizioni]	DATETIME2		NOT NULL,
	[DataCancellazione]			DATETIME2		NULL,		-- Valorizzata se la classe è stata cancellata dalla palestra
	[UserIdOwner]				NVARCHAR(450)	NULL,	-- Utente titolare della classe (se valorizzato è l'unico che può editare la classe)
	[Note]						NVARCHAR(1000)	NULL,
	--timestamp,

	CONSTRAINT PK_Schedules PRIMARY KEY (Id),
	CONSTRAINT FK_Schedules_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_Schedules_Locations FOREIGN KEY (IdLocation) REFERENCES [Locations](Id),
	CONSTRAINT FK_Schedules_TipoLezioni FOREIGN KEY (IdTipoLezione) REFERENCES [TipologieLezioni](Id),
	CONSTRAINT CHK_Schedule_PostiResidui CHECK (PostiResidui >= 0 AND PostiResidui <= PostiDisponibili)
)
