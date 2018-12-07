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
	SET @pSortColumn = COALESCE(@pSortColumn, 'DataOraInizio');
	-- Check colonna di ordinamento (avoid injection)
	IF LOWER(@pSortColumn) NOT IN (N'dataorainizio', N'dataaperturaiscrizioni', N'postidisponibili', N'postiresidui', N'numprenotazioni')
	BEGIN
		RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
		RETURN -1;
	END 

	SET @pSortColumn = QUOTENAME(@pSortColumn)
	SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END
	SET @pPageSize = COALESCE(@pPageSize, 25);
	SET @pPageNumber = COALESCE(@pPageNumber, 1);

	-- Costruiamo la query dinamicamente
	SET @sql = N'	SELECT	s.Id,
							s.IdCliente,
							s.Title,
							s.IdTipoLezione,
							s.IdLocation,
							s.DataOraInizio,
							s.Istruttore,
							s.PostiDisponibili,
							s.PostiResidui,
							(s.PostiDisponibili - PostiResidui) AS NumPrenotazioni,
							s.CancellazioneConsentita,
							s.CancellabileFinoAl,
							s.DataAperturaIscrizioni,
							s.DataChiusuraIscrizioni,
							s.DataCancellazione,
							s.UserIdOwner,
							s.Note,
							s.WaitListDisponibile,
							s.VisibileDal,
							s.VisibileFinoAl,
							COALESCE(s.Recurrency, sp.Recurrency) AS Recurrency,
							s.IdParent,
							l.Nome AS NomeLocation,
							l.CapienzaMax AS CapienzaMaxLocation,
							l.Descrizione AS DescrizioneLocation,
							tl.Nome AS NomeTipoLezione,
							tl.Descrizione AS DescrizioneTipoLezione,
							tl.Durata AS DurataTipoLezione,
							tl.LimiteCancellazioneMinuti AS LimiteCancellazioneMinutiTipoLezione,
							tl.Livello AS LivelloTipoLezione,
							tl.MaxPartecipanti AS MaxPartecipantiTipoLezione,
							tl.DataCreazione AS TipoLezioneDataCreazione,
							tl.DataCancellazione AS TipoLezioneDataCancellazione
					FROM [Schedules] s
						LEFT JOIN [Schedules] sp ON s.IdParent = sp.Id
					INNER JOIN [Locations] l ON s.IdLocation = l.Id
					INNER JOIN [TipologieLezioni] tl ON s.IdTipoLezione = tl.Id
					WHERE s.IdCliente = @pIdCliente'
	IF @pStartDate IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.DataOraInizio >= @pStartDate '		
	END
	IF @pEndDate IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.DataOraInizio <= @DataOraInizio '		
	END
	IF @pIdLocation IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.IdLocation = @pIdLocation '		
	END
	IF @pTipoLezione IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.IdTipoLezione = @pTipoLezione '		
	END
	IF @pSoloPostiDispon IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.PostiResidui > 0 '		
	END
	IF @pSoloIscrizAperte IS NOT NULL
	BEGIN
		SET @sql = @sql + @newLine + 'AND GETDATE() BETWEEN s.DataAperturaIscrizioni AND s.DataChiusuraIscrizioni  '		
	END
	IF COALESCE(@pIncludeDeleted, 0) <> 1
	BEGIN
		SET @sql = @sql + @newLine + 'AND s.DataCancellazione IS NULL '		
	END

	SET @sql = @sql + '		ORDER BY ' + @pSortColumn  + ' ' + @sortDirection + '
							OFFSET @pPageSize * (@pPageNumber - 1) ROWS
							FETCH NEXT @pPageSize ROWS ONLY';

	PRINT @sql
END