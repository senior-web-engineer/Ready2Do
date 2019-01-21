CREATE PROCEDURE [dbo].[Schedules_Lista]
	@pIdCliente			INT,
	@pStartDate			DATETIME2(2) = NULL,
	@pEndDate			DATETIME2(2) = NULL,
	@pIdLocation		INT = NULL,
	@pTipoLezione		INT = NULL,
	@pSoloPostiDispon	BIT = NULL,
	@pSoloIscrizAperte	BIT = NULL,
	@pPageSize			INT = 25,
	@pPageNumber		INT = 1,
	@pSortColumn		VARCHAR(50) = NULL,
	@pOrderAscending	BIT = 1,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
DECLARE @sql NVARCHAR(MAX);
	DECLARE @sortDirection VARCHAR(10);
	DECLARE @newLine CHAR(2) = CHAR(13) + CHAR(10)

	IF @pIdCliente IS NULL
	BEGIN
		RAISERROR('Invalid @pidCliente', 16, 0);
		RETURN -2;		
	END
	SET @pSortColumn = COALESCE(@pSortColumn, 'dataorainizioschedules');
	-- Check colonna di ordinamento (avoid injection)
	IF LOWER(@pSortColumn) NOT IN (N'dataorainizioschedules', N'dataaperturaiscrizionischedules', N'postidisponibilischedules', N'postiresiduischedules', N'numprenotazionischedules')
	BEGIN
		RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
		RETURN -1;
	END 

	SET @pSortColumn = QUOTENAME(@pSortColumn)
	SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END
	SET @pPageSize = COALESCE(@pPageSize, 25);
	SET @pPageNumber = COALESCE(@pPageNumber, 1);

	-- Costruiamo la query dinamicamente
	SET @sql = N'	SELECT	s.*,
							(s.PostiDisponibiliSchedules - s.PostiResiduiSchedules) AS NumPrenotazioniSchedules
					FROM [vSchedulesFull] s						
					WHERE s.IdClienteSchedules = @pIdCliente'
	IF @pStartDate IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.DataOraInizioSchedules >= @pStartDate '		
	END
	IF @pEndDate IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.DataOraInizioSchedules <= @pEndDate '		
	END
	IF @pIdLocation IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.IdLocationSchedules = @pIdLocation '		
	END
	IF @pTipoLezione IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.IdTipoLezioneSchedules = @pTipoLezione '		
	END
	IF @pSoloPostiDispon IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.PostiResiduiSchedules > 0 '		
	END
	IF @pSoloIscrizAperte IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND GETDATE() BETWEEN COALESCE(s.DataAperturaIscrizioniSchedules,''1900-01-01'') AND COALESCE(s.DataChiusuraIscrizioniSchedules,s.DataOraInizioSchedules)  '
	END
	IF COALESCE(@pIncludeDeleted, 0) <> 1
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.DataCancellazioneSchedules IS NULL '		
	END

	SET @sql = @sql + '		ORDER BY ' + @pSortColumn  + ' ' + @sortDirection + '
							OFFSET @pPageSize * (@pPageNumber - 1) ROWS
							FETCH NEXT @pPageSize ROWS ONLY';
	
	--PRINT @sql
	EXEC sp_executesql @sql, N'@pPageSize int, @pPageNumber int, @pIdCliente int, @pStartDate datetime2, @pEndDate datetime2, @pIdLocation int, @pTipoLezione int', 
						@pStartDate=@pStartDate, @pEndDate=@pEndDate, @pIdLocation=@pIdLocation, @pTipoLezione=@pTipoLezione, @pPageSize=@pPageSize, @pPageNumber=@pPageNumber, @pIdCliente=@pIdCliente
END