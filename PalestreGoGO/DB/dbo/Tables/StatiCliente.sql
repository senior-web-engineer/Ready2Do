/*
Possibili stati in cui può trovarsi un Cliente.
0:  NotProvisioned (appena creato) ==> Cliente non ancora visibile a sistema
3:  Provisioned - Mail Not Confirmed ==> Cliente visibile solo dall'owner
10: Confirmed ==> email confermata, tutte le funzionalità sono disponibili
*/
CREATE TABLE [dbo].[StatiCliente]
(
	[IdStato]		TINYINT			NOT NULL,
	[Nome]			VARCHAR(50)		NOT NULL,
	[Descrizione]	VARCHAR(300)	NOT NULL,
	
	CONSTRAINT PK_StatiClienti PRIMARY KEY (IdStato)
)
