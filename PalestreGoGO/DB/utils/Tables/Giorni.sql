CREATE TABLE [utils].[Giorni]
(
	[Data]				DATE NOT NULL,
	[Year]				INT  NOT NULL,
	[Month]				INT	 NOT NULL,
	[WeekNum]			INT  NOT NULL,
	[DayOfWeekNum]		INT  NOT NULL,
	[DayOfWeekEng]		VARCHAR(50) NOT NULL,
	[DayOfWeekIta]		VARCHAR(50) NOT NULL,
	[DayOfWeekItaShort]	CHAR(3) NOT NULL,


	CONSTRAINT PK_Giorni PRIMARY KEY ([Data])
)
GO

CREATE NONCLUSTERED INDEX IDX_Giorni_DayOfWeekData ON [utils].[Giorni] ([DayOfWeekNum],[Data])
