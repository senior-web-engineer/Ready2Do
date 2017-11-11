/*
Gli utenti sono gestiti esternamente ed abbiamo solo un identificativo.
Per ottenere l'anagrafica è necessario invocare l'endpoint dedicato

NOTA: Valutare se replicare alcune informazioni anagrafiche con il rischio di disallinemanto dei dati		
*/
CREATE TABLE [dbo].[AbbonamentiUtenti]
(
	[Id]						INT				NOT NULL	IDENTITY(1,1),
	[IdCliente]					INT				NOT NULL,
	[UserId]					NVARCHAR(450)	NOT NULL,	--L'utente è esterno
	[IdTipoAbbonamento]			INT				NOT NULL,
	[DataInizioValidita]		DATE			NOT NULL,	-- Ma serve?
	[Scadenza]					DATE			NOT NULL,
	[IngressiResidui]			SMALLINT		NULL,
	[ScadenzaCertificato]		DATE			NULL,
	[StatoPagamento]			TINYINT			NULL,	--??

	CONSTRAINT PK_AbbonamentiUtenti PRIMARY KEY (Id),
	CONSTRAINT FK_AbbonamentiUtenti_Clienti FOREIGN KEY(IdCliente) REFERENCES [Clienti](Id),
	CONSTRAINT FK_AbbonamentiUtenti_TipoAbbonamento FOREIGN KEY (IdTipoAbbonamento) REFERENCES [TipologieAbbonamenti](Id)

)
