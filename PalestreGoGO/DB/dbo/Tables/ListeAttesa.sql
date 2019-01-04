/*
Gestione della lista d'attesa per gli eventi.
ATTENZIONE! E' disponibile solo per gli utenti Registrati (per cui avranno sempre un Abbonamento associato)
NOTE:
- CausaleCancellazione: 1 = Utente, 2 = Owner, 3 = Expired
*/
CREATE TABLE [dbo].[ListeAttesa]
(
	[Id]					INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]				INT				NOT NULL,
	[IdSchedule]			INT				NOT NULL,
	[UserId]				VARCHAR(100)	NOT NULL,
	[IdAbbonamento]			INT				NOT NULL,
	[DataCreazione]			DATETIME2(2)	NOT NULL CONSTRAINT DEF_ListeAttesa_DataCreaz DEFAULT(sysdatetime()),
	[DataScadenza]			DATETIME2(2)	NOT NULL, -- Data ultima di validità della registrazione in lista d'attesa
	[DataConversione]		DATETIME2(2)	NULL, -- Valorizzato nel momento in cui viene trasformato in appuntamento
	[DataCancellazione]		DATETIME2(2)	NULL, --Cancellazione dalla lista d'attesa (annullamento)
	[CausaleCancellazione]	TINYINT			NULL CONSTRAINT DEF_ListeAttesa_CausCanc CHECK([CausaleCancellazione] IN (1,2,3)),
	CONSTRAINT PK_ListeAttesa PRIMARY KEY (Id),
	INDEX IDX_ListeAttesaDataCreaz NONCLUSTERED (IdSchedule, DataCreazione),
	-- Al più una registrazione per utente per schedule non cancellata (pending o convertita)
	--ATTENZIONE! Questo implica che un utente che finisce in lista d'attesa e viene convertito, se annulla l'iscrizione non può rimettersi in lista d'attesa!
	INDEX UQIDX_ListeAttesa_UserIdSchedule UNIQUE(IdSchedule, UserId) WHERE DataCancellazione IS NULL
)
