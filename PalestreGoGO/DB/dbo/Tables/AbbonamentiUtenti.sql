/*
Gli utenti sono gestiti esternamente ed abbiamo solo un identificativo.

*/
CREATE TABLE [dbo].[AbbonamentiUtenti]
(
	[Id]						INT					NOT NULL	IDENTITY(1,1),
	[IdCliente]					INT					NOT NULL,
	[UserId]					VARCHAR(50)			NOT NULL,	--L'utente è esterno
	[IdTipoAbbonamento]			INT					NOT NULL,
	[DataInizioValidita]		DATETIME2(2)		NOT NULL,	-- Ma serve?
	[Scadenza]					DATETIME2(2)		NOT NULL,
	[IngressiIniziali]			SMALLINT			NULL		CONSTRAINT CHK_IngressiIniz_NotNeg CHECK(COALESCE([IngressiIniziali],0) >= 0),	
	[IngressiResidui]			SMALLINT			NULL		CONSTRAINT CHK_IngressiRes_NotNeg CHECK(COALESCE([IngressiResidui],0) >= 0),
	[Importo]					DECIMAL(10,2)		NOT NULL,
	[ImportoPagato]				DECIMAL(10,2)		NOT NULL,
	[DataCreazione]				DATETIME2(2)		NOT NULL	CONSTRAINT DEF_AbbonamentiUtenti_DtCreazione DEFAULT (SYSDATETIME()),
	[DataCancellazione]			DATETIME2(2)		NULL,

	CONSTRAINT PK_AbbonamentiUtenti PRIMARY KEY (Id),
	CONSTRAINT FK_AbbonamentiUtenti_Clienti FOREIGN KEY(IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_AbbonamentiUtenti_TipoAbbonamento FOREIGN KEY (IdTipoAbbonamento) REFERENCES [TipologieAbbonamenti](Id),
	-- Nella versiona attuale un utente può avere solo un abbonamento, in futuro sarà usata la tabella ClientiUtentiAbbonamenti per gestire lo storico.
	-- A quel punto sarà necessario rimuovere questo constraint
	--CONSTRAINT UQ_AbbonamentiUtenti_UtenteCliente UNIQUE (IdCliente, UserId) 
)
