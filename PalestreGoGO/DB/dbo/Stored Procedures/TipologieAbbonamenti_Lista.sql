CREATE PROCEDURE [dbo].[TipologieAbbonamenti_Lista]
	@pIdCliente			INT,
	@pId				INT = NULL,
	@pPageSize			INT = 25,
	@pPageNumber		INT = 1,
	@pSortColumn		VARCHAR(50) = NULL,
	@pOrderAscending	BIT = 1,
	@pIncludeDeleted	BIT = 0,
	@pIncludeNotActive	BIT = 0,
	@pDatetimeInterest	DATETIME2(2) = NULL, -- Se specificata usa questa data per determinare se un abbonamento è attivo, altrimenti usa la SYSDATETIME()
	@pTotalNumRecords	INT OUTPUT	--Solo per la prima pagina contiene il numero di record totali
AS
BEGIN
	DECLARE @newLine CHAR(2) = CHAR(13)+ CHAR(10)
	DECLARE @sql NVARCHAR(MAX);
	DECLARE @sql_count NVARCHAR(MAX);
	DECLARE @whereConditions NVARCHAR(MAX);
	DECLARE @sortDirection VARCHAR(10);

	IF @pIdCliente IS NULL
	BEGIN
		RAISERROR('Invalid @pIdCliente', 16, 0);
		RETURN -2;		
	END

	SET @pDatetimeInterest = COALESCE(@pDatetimeInterest, SYSDATETIME())

	-- Check colonna di ordinamento (avoid injection)
	IF LOWER(@pSortColumn) NOT IN (N'nome', N'descrizione', N'duratamesi', N'numingressi', N'costo', N'maxlivcorsi', N'validodal', N'validofinoal', N'datacreazione', N'datacancellazione')
	BEGIN
		RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
		RETURN -1;
	END 
  SET @pSortColumn = QUOTENAME(@pSortColumn)
  SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END
  SET @pPageSize = COALESCE(@pPageSize, 25);
  SET @pPageNumber = COALESCE(@pPageNumber, 1);
  
  SET @sql = N'SELECT
					   t.Id, 
					   t.IdCliente,
					   t.Nome,
					   t.DurataMesi,
					   t.NumIngressi,
					   t.Costo,
					   t.MaxLivCorsi,
					   t.ValidoDal,
					   t.ValidoFinoAl,
					   t.DataCreazione,
					   t.DataCancellazione
				FROM TipologieAbbonamenti t'

	-- Costruiamo le WHERE Conditions separatamente per poterle riutilizzare con la COUNT(*)
	SET @whereConditions = 'WHERE t.IdCliente = @pIdCliente ' + @newLine
	
	IF @pId IS NOT NULL 
	BEGIN
		SET @whereConditions += 'AND t.Id = @pId ' + @newLine
	END
	IF COALESCE(@pIncludeDeleted, 0) <> 1
	BEGIN
		SET @whereConditions += 'AND t.DataCancellazione IS NULL ' + @newLine
	END
	IF COALESCE(@pIncludeNotActive, 0) <> 1
	BEGIN
		SET @whereConditions += 'AND @pDatetimeInterest BETWEEN t.ValidoDal AND COALESCE(t.ValidoFinoAl, ''9999-12-31'') ' + @newLine
	END
	-- Per la prima pagina andiamo a contare i record totali ritornati dalla query senza paginazione
	IF @pPageNumber = 1
	BEGIN
		SET @sql_count = 'SELECT @numRecords = COUNT(t.Id) FROM TipologieAbbonamenti t ' + @newLine +  @whereConditions
		EXEC sp_executesql @sql_count, N'@pIdCliente int, @pDatetimeInterest datetime2(2), @pId INT, @numRecords int OUTPUT',
			 @pIdCliente=@pIdCliente, @pDatetimeInterest= @pDatetimeInterest,@pId=@pId,@numRecords=@pTotalNumRecords
	END
	
	SET @pTotalNumRecords = COALESCE(@pTotalNumRecords, 0)

	--Eseguiamo la query paginata
	SET @sql = @sql + @newLine + @whereConditions + @newLine +
			   'ORDER BY ' + COALESCE(@pSortColumn, 'DataCreazione') + ' ' + @sortDirection + ' 
			    OFFSET @pPageSize * (@pPageNumber - 1) ROWS
				FETCH NEXT @pPageSize ROWS ONLY';
				
	EXEC sp_executesql @sql, N'@pPageSize int, @pPageNumber int, @pIdCliente int, @pDatetimeInterest datetime2(2),  @pId INT', 
				@pPageSize=@pPageSize, @pPageNumber=@pPageNumber, @pIdCliente=@pIdCliente, @pDatetimeInterest= @pDatetimeInterest, @pId =@pId

	RETURN 0
END
