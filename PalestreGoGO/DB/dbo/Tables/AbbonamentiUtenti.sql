/*
Gli utenti sono gestiti esternamente ed abbiamo solo un identificativo.
Per ottenere l'anagrafica è necessario invocare l'endpoint dedicato

NOTA: Valutare se replicare alcune informazioni anagrafiche con il rischio di disallinemanto dei dati		
*/
CREATE TABLE [dbo].[AbbonamentiUtenti]
(
	[Id]						INT					NOT NULL	IDENTITY(1,1),
	[IdCliente]					INT					NOT NULL,
	[UserId]					UNIQUEIDENTIFIER	NOT NULL,	--L'utente è esterno
	[IdTipoAbbonamento]			INT					NOT NULL,
	[DataInizioValidita]		DATE				NOT NULL,	-- Ma serve?
	[Scadenza]					DATE				NOT NULL,
	[IngressiResidui]			SMALLINT			NULL,
	[ScadenzaCertificato]		DATE				NULL,
	[StatoPagamento]			TINYINT				NULL,	--??
	[DataCreazione]				DATETIME2(2)		NOT NULL	CONSTRAINT DEF_AbbonamentiUtenti_DtCreazione DEFAULT (SYSDATETIME()),

	CONSTRAINT PK_AbbonamentiUtenti PRIMARY KEY (Id),
	CONSTRAINT FK_AbbonamentiUtenti_Clienti FOREIGN KEY(IdCliente) REFERENCES [Clienti]([Id]),
	CONSTRAINT FK_AbbonamentiUtenti_TipoAbbonamento FOREIGN KEY (IdTipoAbbonamento) REFERENCES [TipologieAbbonamenti](Id),
	-- Nella versiona attuale un utente può avere solo un abbonamento, in futuro sarà usata la tabella ClientiUtentiAbbonamenti per gestire lo storico.
	-- A quel punto sarà necessario rimuovere questo constraint
	CONSTRAINT UQ_AbbonamentiUtenti_UtenteCliente UNIQUE (IdCliente, UserId) 
)
