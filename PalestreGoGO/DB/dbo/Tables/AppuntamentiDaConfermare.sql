CREATE TABLE [dbo].[AppuntamentiDaConfermare]
(
	[Id]					INT					NOT NULL	IDENTITY(1,1),
	[IdCliente]				INT					NOT NULL,
	[UserId]				VARCHAR(100)		NULL,		--Se NULL indica un appuntamento per un utente Guest
	[ScheduleId]			INT					NOT NULL,
	[DataCreazione]			DATETIME2(2)		NOT NULL	CONSTRAINT DEF_AppuntamentiDaConfermare_DataCreaz DEFAULT(SYSDATETIME()),
	[DataEsito]				DATETIME2(2)		NULL,
	[IdAppuntamento]		INT					NULL,		--Per gli appuntamenti confermati ci salvimo l'appuntamento corrispondente
	[MotivoRifiuto]			NVARCHAR(MAX)		NULL,		--Solo per gli appuntamenti rifiutati ci salviamo il motivo (per distinguere i timeout dagli altri)
	
	CONSTRAINT PK_AppuntamentiDaConfermare PRIMARY KEY (Id),
	CONSTRAINT FK_AppuntamentiDaConfermare_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_AppuntamentiDaConfermare_Schedules FOREIGN KEY (ScheduleId) REFERENCES [Schedules](Id),

)
