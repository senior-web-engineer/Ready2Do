CREATE TABLE [dbo].[Notifiche]
(
	[Id]							BIGINT			NOT NULL IDENTITY (1,1),
	[IdTipo]						INT				NOT NULL,
	[UserId]						VARCHAR(100)	NOT NULL,
	[IdCliente]						INT				NULL,	--Valorizzato SOLO se la notifica è relativa ad uno specifico Cliente
	[Titolo]						NVARCHAR(50)	NOT NULL,
	[Testo]							NVARCHAR(1000)	NOT NULL,
	[ActionUrl]						VARCHAR(5000)	NULL,
	[DataCreazione]					DATETIME2(2)	NOT NULL CONSTRAINT DEF_NotificheDataCreaz DEFAULT (SYSDATETIME()),
	[DataInizioVisibilita]			DATETIME2(2)	NULL,
	[DataFineVisibilita]			DATETIME2(2)	NULL,
	[DataDismissione]				DATETIME2(2)	NULL,
	[DataPrimaVisualizzazione]		DATETIME2(2)	NULL,

	CONSTRAINT PK_Notifiche PRIMARY KEY(Id),
	CONSTRAINT FK_Notifiche_TipologieNotifiche FOREIGN KEY (IdTipo) REFERENCES TipologieNotifiche(Id),
)
