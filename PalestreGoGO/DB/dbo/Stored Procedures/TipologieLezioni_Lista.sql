CREATE PROCEDURE [dbo].[TipologieLezioni_Lista]
	@pIdCliente			INT,
	@pId				INT = NULL,
	@pPageSize			INT = 25,
	@pPageNumber		INT = 1,
	@pSortColumn		VARCHAR(50) = NULL,
	@pOrderAscending	BIT = 1
AS
BEGIN
	DECLARE @sql VARCHAR(MAX);
	DECLARE @sortDirection VARCHAR(10);
	-- Check colonna di ordinamento (avoid injection)
	  IF LOWER(@pSortColumn) NOT IN (N'nome', N'descrizione', N'durata', N'maxpartecipanti', N'limitecancellazioneminuti', N'livello', N'datacreazione')
  BEGIN
    RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
    RETURN -1;
  END 

  SET @pSortColumn = QUOTENAME(@pSortColumn)
  SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END

  SET @sql = N'SELECT t.Id, 
					   t.IdCliente,
					   t.DataCancellazione,
					   t.DataCreazione,
					   t.Descrizione,
					   t.Durata,
					   t.LimiteCancellazioneMinuti,
					   t.Livello,
					   t.MaxPartecipanti,
					   t.Nome
				FROM TipologieLezioni t
				' + CASE WHEN @pId IS NULL THEN ' ' ELSE ' WHERE t.Id = ' END + CAST(@pId AS VARCHAR(50)) +'
				ORDER BY ' + @pSortColumn + ' ' + @sortDirection + ' 
			    OFFSET @pPageSize * (@pPageNumber - 1) ROWS
				FETCH NEXT @pPageSize ROWS ONLY';

	EXEC sp_executesql @sql;

	RETURN 0
END
