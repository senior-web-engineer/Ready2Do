CREATE TABLE [utils].[Numbers]
(
	[Number]	INT		NOT NULL
	
	CONSTRAINT PK_Numbers  PRIMARY KEY CLUSTERED (Number) WITH 
	(
	  FILLFACTOR = 100,      -- in the event server default has been changed
	  DATA_COMPRESSION = ROW -- if Enterprise & table large enough to matter
	)
)
GO