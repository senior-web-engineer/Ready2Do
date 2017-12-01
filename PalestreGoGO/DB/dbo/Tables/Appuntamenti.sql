CREATE TABLE [dbo].[Appuntamenti]
(
	[Id]					INT					NOT NULL	IDENTITY(1,1),
	[IdCliente]				INT					NOT NULL,
	[UserId]				UNIQUEIDENTIFIER	NOT NULL,
	[ScheduleId]			INT					NOT NULL,
	[DataPrenotazione]		DATETIME2			NOT NULL,
	[DataCancellazione]		DATETIME2			NULL,
	[Note]					NVARCHAR(1000)		NULL,
	[Nominativo]			NVARCHAR(200)		NULL,
	[IsGuest]				BIT					NOT NULL,
	CONSTRAINT PK_Appuntamenti PRIMARY KEY (Id),
	CONSTRAINT FK_Appuntamenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_Appuntamenti_Schedules FOREIGN KEY (ScheduleId) REFERENCES [Schedules](Id),
	-- Per un utente può esserci solo un appuntamento NON CANCELLATO (ATTIVO)
	INDEX IDX_Appuntamenti_UserSched UNIQUE (UserId, ScheduleId) WHERE [DataCancellazione] IS NULL
)
