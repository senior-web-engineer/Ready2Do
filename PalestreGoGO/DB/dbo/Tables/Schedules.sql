/*
####### NOTE #####
* DataChiusuraIscrizioni: Indica la data oltre la quale per uno schedule non è consentito agli utenti di iscriversi "direttamente", è però possibile
*						  che a seguito di una cancellazione un utente in WaitingList venga iscritto all'evento dopo questa data dal sistema
*/
CREATE TABLE [dbo].[Schedules]
(
	[Id]						INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]					INT				NOT NULL,
	[Title]						NVARCHAR(100)	NOT NULL,
	[IdTipoLezione]				INT				NOT NULL,
	[IdLocation]				INT				NOT NULL,
	[DataOraInizio]				DATETIME2(2)	NOT NULL,
	[Istruttore]				NVARCHAR(150)	NULL,
	[PostiDisponibili]			INT				NOT NULL,	-- Posti inizilamente disponibili
	[PostiResidui]				INT				NOT NULL,	-- Contatore dei posti ancora disponibili
	[CancellazioneConsentita]	BIT				NOT NULL,
	[CancellabileFinoAl]		DATETIME2(2)	NULL,
	[DataAperturaIscrizioni]	DATETIME2(2)	NULL,	-- Momento da cui è possibile iniziare ad iscriversi
	[DataChiusuraIscrizioni]	DATETIME2(2)	NULL,
	[DataCancellazione]			DATETIME2(2)	NULL,		-- Valorizzata se la classe è stata cancellata dalla palestra
	[UserIdOwner]				NVARCHAR(450)	NULL,	-- Utente titolare della classe (se valorizzato è l'unico che può editare la classe)
	[Note]						NVARCHAR(1000)	NULL,
	[WaitListDisponibile]		BIT				NOT NULL,
	[VisibileDal]				DATETIME2(2)	NULL,   -- indica da quando lo schedule e pubblicamente visibile
	[VisibileFinoAl]			DATETIME2(2)	NULL,	-- Indica fino a quando lo schedule è visibile pubblicamente
	--20181201-Recurrent Events
	[Recurrency]				NVARCHAR(MAX)	NULL,
	[IdParent]					INT				NULL,
	CONSTRAINT PK_Schedules PRIMARY KEY (Id),
	CONSTRAINT FK_Schedules_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_Schedules_Locations FOREIGN KEY (IdLocation) REFERENCES [Locations](Id),
	CONSTRAINT FK_Schedules_TipoLezioni FOREIGN KEY (IdTipoLezione) REFERENCES [TipologieLezioni](Id),
	CONSTRAINT CHK_Schedule_PostiResidui CHECK (PostiResidui >= 0 AND PostiResidui <= PostiDisponibili),
	CONSTRAINT FK_Schedules_IdParent FOREIGN KEY (IdParent) REFERENCES [Schedules]([Id])
)
