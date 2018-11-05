CREATE TABLE [dbo].[Notifiche]
(
	[Id]				BIGINT			NOT NULL IDENTITY (1,1),
	[IdTipo]			INT				NOT NULL,
	[IdUtente]			VARCHAR(100)	NOT NULL,
	[IdCliente]			INT				NULL,	--Valorizzato SOLO se la notifica è relativa ad uno specifico Cliente
	[DataCreazione]		DATETIME2(2)	NOT NULL CONSTRAINT DEF_NotificheDataCreaz DEFAULT (SYSDATETIME()),
	

	CONSTRAINT PK_Notifiche PRIMARY KEY(Id),
	CONSTRAINT FK_Notifiche_TipologieNotifiche FOREIGN KEY (IdTipo) REFERENCES TipologieNotifiche(Id),
)
