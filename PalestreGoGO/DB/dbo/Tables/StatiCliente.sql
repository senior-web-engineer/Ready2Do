/*
Possibili stati in cui può trovarsi un Cliente.
0:  NotProvisioned (appena creato) ==> Cliente non ancora visibile a sistema
3:  Provisioned - Struttura creata ma visibile solo dall'owner (viene creata in questo stato quando l'utente non ha confermato l'email
10: Published  ==> Struttura pubblicamente visibile
NOTE:
Un struttura una volta terminato il provisioning viene messa in stato 3 (provisioned) se l'uente owner non ha confermato la propria email.
Solo quando l'utente confermerà il proprio account la struttura passerà in stato 10 (published)
In futuro possiamo gestire anche la possibilità di aggiungere stati intermedi (Unpublished) per consentire all'owner di rimuovere la pubblicazione
*/
CREATE TABLE [dbo].[StatiCliente]
(
	[IdStato]		TINYINT			NOT NULL,
	[Nome]			VARCHAR(50)		NOT NULL,
	[Descrizione]	VARCHAR(300)	NOT NULL,
	
	CONSTRAINT PK_StatiClienti PRIMARY KEY (IdStato)
)
