/*
-- History --
2018.11.09#GT#
Aggiunta colonna IdAbbonamento per tenere traccia dell'abbonamente da cui è stato scalato l'ingresso per gestire l'eventuale riaccredito in fase di cancellazione.
Sebbene possa sembrare una denormalizzazione mantenere anche lo UserId (dato che un abbonamento implica un utente), si è deciso di procedere in questo modo per gestire
anche eventuali scenari di "Iscrizioni Gratuite" per cui abbiamo un Utente ma non viene scalato nessun ingresso (IdAbbonamento = NULL)
*/
CREATE TABLE [dbo].[Appuntamenti]
(
	[Id]					INT					NOT NULL	IDENTITY(1,1),
	[IdCliente]				INT					NOT NULL,
	[UserId]				VARCHAR(100)		NULL,		--Se NULL indica un appuntamento per un utente Guest
	[ScheduleId]			INT					NOT NULL,
	[IdAbbonamento]			INT					NULL,		--Abbonamento da cui è stato scalato l'ingresso o cmq utilizzato per la prenotazione
	[DataPrenotazione]		DATETIME2			NOT NULL,
	[DataCancellazione]		DATETIME2			NULL,
	[Note]					NVARCHAR(1000)		NULL,
	[Nominativo]			NVARCHAR(200)		NULL,	/* ??? */
	CONSTRAINT PK_Appuntamenti PRIMARY KEY (Id),
	CONSTRAINT FK_Appuntamenti_Clienti FOREIGN KEY (IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_Appuntamenti_Schedules FOREIGN KEY (ScheduleId) REFERENCES [Schedules](Id),
	CONSTRAINT FK_Appuntamenti_Abbonamenti FOREIGN KEY (IdAbbonamento) REFERENCES [AbbonamentiUtenti](Id),
	-- Per un utente può esserci solo un appuntamento NON CANCELLATO (ATTIVO)
	INDEX IDX_Appuntamenti_UserSched UNIQUE (UserId, ScheduleId) WHERE [DataCancellazione] IS NULL,

	INDEX IDX_AppuntamentiSchedules NONCLUSTERED (ScheduleId)
)
