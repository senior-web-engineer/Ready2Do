/*
Questa tabella serve per gestire le richieste di registrazione degli UTENTI (non necessariamente clienti) e per la conferma
degli stessi tramite il codice univoco inviato alla loro email
Il campo CorrelationId è usato, nel caso di Utente Owner di struttura (Cliente) per associare la richiesta al Cliente stesso
così da poter cambiare lo stato del Cliente alla conferma dell'Utente Owner.
Se questo campo è NULL implica che per l'utente cui appartiene la richiesta non esiste un Cliente associato (Utente ordinario)
*/
CREATE TABLE [dbo].[RichiesteRegistrazione]
(
	Id					INT					NOT NULL	IDENTITY(1,1),
	DataRichiesta		DATETIME2(2)		NOT NULL	CONSTRAINT DEF_DataRichiestaRegistrazione DEFAULT SYSDATETIME(),
	CorrelationId		UNIQUEIDENTIFIER	NULL,
	UserCode			VARCHAR(1000)		NOT NULL,
	UserName			VARCHAR(500)		NOT NULL,
	Expiration			DATETIME2(2)		NOT NULL,
	Refereer			INT					NULL,
	DataConferma		DATETIME2(2)		NULL,
	DataCancellazione	DATETIME2(2)		NULL, --Gestiamo la cancellazione logica per tenere traccia degli eventuali timeout durante la registrazione

	CONSTRAINT PK_RichiesteRegistrazioni PRIMARY KEY (Id),
	--Per un utente può esistere solo una richiesta di registrazione non cancellata
	INDEX IDXUQ_RichiesteRegistrazioni_Username UNIQUE (Username) WHERE DataCancellazione IS NULL
)
