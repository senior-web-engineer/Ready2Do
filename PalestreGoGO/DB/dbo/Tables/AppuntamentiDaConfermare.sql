CREATE TABLE [dbo].[AppuntamentiDaConfermare]
(
	[Id]					INT					NOT NULL	IDENTITY(1,1),
	[IdCliente]				INT					NOT NULL,
	[UserId]				VARCHAR(100)		NOT NULL,	
	[ScheduleId]			INT					NOT NULL,
	[DataCreazione]			DATETIME2(2)		NOT NULL	CONSTRAINT DEF_AppuntamentiDaConfermare_DataCreaz DEFAULT(SYSDATETIME()),
	[DataExpiration]		DATETIME2(2)		NOT NULL,	-- Soglia oltre cui si considera automaticamente rifiutato
	[DataEsito]				DATETIME2(2)		NULL,
	[IdAppuntamento]		INT					NULL,		--Per gli appuntamenti confermati ci salvimo l'appuntamento corrispondente
	[MotivoRifiuto]			NVARCHAR(MAX)		NULL,		--Solo per gli appuntamenti rifiutati ci salviamo il motivo (per distinguere i timeout dagli altri)
	[DataCancellazione]		DATETIME2(2)		NULL,		--Eventuale data annullamento
	[TimeoutManagerPayload] NVARCHAR(MAX)		NULL,

	CONSTRAINT PK_AppuntamentiDaConfermare PRIMARY KEY (Id),
	CONSTRAINT FK_AppuntamentiDaConfermare_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_AppuntamentiDaConfermare_Schedules FOREIGN KEY (ScheduleId) REFERENCES [Schedules](Id),
	--Al più un appuntamento da confermare per utente/evento (NON CANCELLATO)
	INDEX UQ_AppuntamentiDaConfermare_ScheduleUser UNIQUE (ScheduleId, UserId) WHERE DataCancellazione IS NULL,
)
