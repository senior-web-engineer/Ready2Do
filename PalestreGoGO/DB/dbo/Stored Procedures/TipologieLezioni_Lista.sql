CREATE PROCEDURE [dbo].[TipologieLezioni_Lista]
	@pIdCliente			INT,
	@pId				INT = NULL,
	@pPageSize			INT = 25,
	@pPageNumber		INT = 1,
	@pSortColumn		VARCHAR(50) = NULL,
	@pOrderAscending	BIT = 1
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX);
	DECLARE @sortDirection VARCHAR(10);
	IF @pIdCliente IS NULL
	BEGIN
		RAISERROR('Invalid @pidCliente', 16, 0);
		RETURN -2;		
	END
	-- Check colonna di ordinamento (avoid injection)
	IF LOWER(@pSortColumn) NOT IN (N'nome', N'descrizione', N'durata', N'maxpartecipanti', N'limitecancellazioneminuti', N'livello', N'datacreazione')
	BEGIN
		RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
		RETURN -1;
	END 
  SET @pSortColumn = QUOTENAME(@pSortColumn)
  SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END
  SET @pPageSize = COALESCE(@pPageSize, 25);
  SET @pPageNumber = COALESCE(@pPageNumber, 1);

  SET @sql = N'SELECT  *
				FROM vTipologieLezioni t
				WHERE t.IdCliente = @pIdCliente
				' + CASE WHEN @pId IS NULL THEN ' ' ELSE ' AND t.Id = ' + CAST(@pId AS VARCHAR(50)) END  +'
				ORDER BY ' + COALESCE(@pSortColumn, 'DataCreazione') + ' ' + @sortDirection + ' 
			    OFFSET @pPageSize * (@pPageNumber - 1) ROWS
				FETCH NEXT @pPageSize ROWS ONLY';
				
	EXEC sp_executesql @sql, N'@pPageSize int, @pPageNumber int, @pIdCliente int', @pPageSize=@pPageSize, @pPageNumber=@pPageNumber, @pIdCliente=@pIdCliente

	RETURN 0
END
