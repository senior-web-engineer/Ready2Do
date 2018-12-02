--truncate table utils.Giorni
--go

CREATE TABLE #appoGiorni
(
	[Data]				DATE NOT NULL,
	[Year]				INT  NOT NULL,
	[Month]				INT	 NOT NULL,
	[WeekNum]			INT  NOT NULL,
	[DayOfWeekNum]		INT  NOT NULL,
	[DayOfWeekEng]		VARCHAR(50) NOT NULL,
	[DayOfWeekIta]		VARCHAR(50) NULL,
	[DayOfWeekItaShort]	CHAR(3) NULL,
)

SET LANGUAGE us_english
DECLARE @startDate DATE = '2018-01-01'
;with cte_date as (
	SELECT DATEADD(day, t.RowNum -1, @startDate) AS [Data]
	  FROM 
	(
	SELECT ROW_NUMBER() OVER (ORDER BY o1.object_id, o2.object_id) AS RowNum
	FROM sys.objects o1
	 cross join sys.objects o2
	)T
)
INSERT INTO #appoGiorni([Data], [Year], [Month], [WeekNum], [DayOfWeekNum], [DayOfWeekEng])
SELECT [Data], DATEPART(year, [Data]), DATEPART(month, [Data]), DATEPART(week, [Data]), DATEPART(dw, [Data]), DATENAME(dw, [Data]) from cte_date

SET LANGUAGE Italian
UPDATE #appoGiorni
	SET [DayOfWeekIta] = DATENAME(dw, [Data]),
		[DayOfWeekItaShort] = LEFT(DATENAME(dw, [Data]), 3)

INSERT INTO utils.Giorni ([Data], [Year], [Month], [WeekNum], [DayOfWeekNum], [DayOfWeekEng], [DayOfWeekIta], [DayOfWeekItaShort])
	SELECT [Data], [Year], [Month], [WeekNum], [DayOfWeekNum], [DayOfWeekEng], [DayOfWeekIta], [DayOfWeekItaShort] FROM #appoGiorni
	WHERE [Data] < '2060-01-01'
--select * from utils.Giorni