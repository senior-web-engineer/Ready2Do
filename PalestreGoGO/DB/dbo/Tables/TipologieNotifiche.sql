CREATE TABLE [dbo].[TipologieNotifiche]
(
	[Id]					INT				NOT NULL	IDENTITY(1,1),
	[Code]					VARCHAR(50)		NOT NULL,
	[UserDismissable]		BIT				NOT NULL,
	/*Numero di secondi trascorsi i quali una notifica di questo tipo si considera automaticamente dismissed*/
	[AutoDismissAfter]		BIGINT			NULL,
	[Priority]				INT				NOT NULL	CONSTRAINT DEF_TipologieNotifichePriority DEFAULT(0),

	CONSTRAINT PK_TipologieNotifiche PRIMARY KEY (Id),
	CONSTRAINT UQ_TipologieNotifiche_Code UNIQUE (Code),
)
