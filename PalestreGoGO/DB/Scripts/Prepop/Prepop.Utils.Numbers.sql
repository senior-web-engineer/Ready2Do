DECLARE @UpperBound INT = 1000000;

;WITH cteN(Number) AS
(
  SELECT ROW_NUMBER() OVER (ORDER BY s1.[object_id]) - 1
  FROM sys.all_columns AS s1
  CROSS JOIN sys.all_columns AS s2
)
SELECT [Number] INTO [utils].[Numbers]
FROM cteN WHERE [Number] <= @UpperBound;
