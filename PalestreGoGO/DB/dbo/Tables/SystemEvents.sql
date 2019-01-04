/*
Tabella utilizzata per gestire la "notifica" degli eventi avvenuti a sistema.
Dato che da SQL non riusciamo a generare eventi in Event Grid (o altri meccenismi di gestione degli eventi), quando si palesa l'esigenza di
pubblicare un evento scatanato dal codice SQL (presumibilmente perchè non generabile esternamente a causa dei requisiti transazionali),
viene generato un record in questa tabella su cui verrà fatto polling periodicamente.
*/
CREATE TABLE [dbo].[SystemEvents]
(
	[EventId]			UNIQUEIDENTIFIER	NOT NULL CONSTRAINT DEF_SystemEvents_Id DEFAULT(NEWSEQUENTIALID()),
	[EventType]			VARCHAR(500)		NOT NULL, -- Stringa identificativa della tipologia di evento
	[EventSubType]		VARCHAR(500)		NULL,	  -- Stringa identificativa della sotot tipologia di evento
	[EventPayload]		NVARCHAR(MAX)		NOT NULL,
	[CreationDate]		DATETIME2			NOT NULL CONSTRAINT DEF_SystemEvents_Creation DEFAULT(SYSDATETIME()),
	[StartProcessing]	DATETIME2			NULL,
	[EndProcessing]		DATETIME2			NULL,

	CONSTRAINT PK_SystemEvents PRIMARY KEY (EventId),
	-- Il payload deve essere un json valido
	CONSTRAINT CHK_SystemEvents_Payload_Json CHECK(ISJSON([EventPayload]) = 1)
--	CONSTRAINT CHK_SystemEvents_EndProcStartProc CHECK (COALESCE(EndProcessing,'9999-12-31') >= COALESCE(StartProcessing, '1900-01-01'))
)
