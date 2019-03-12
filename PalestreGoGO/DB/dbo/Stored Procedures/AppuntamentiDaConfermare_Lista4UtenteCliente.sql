CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Lista4UtenteCliente]
	@pIdCliente				INT, --Opzionale, se non passato ritorna tutti gli appuntamenti dell'uitente a prescindere dal Cliente
	@pUserId				VARCHAR(100),	
	@pScheduleStartDate		DATETIME2(2) = NULL,
	@pScheduleEndDate		DATETIME2(2) = NULL,
	@pIncludeDeleted		bit = 0,
	@pIncludeExpired		bit = 0,
	@pPageSize				INT = 25,
	@pPageNumber			INT = 1,
	@pSortColumn			VARCHAR(50) = NULL,
	@pOrderAscending		BIT = 1
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX);
	DECLARE @sortDirection VARCHAR(10);
	DECLARE @newLine CHAR(2) = CHAR(13)+ CHAR(10)

	IF @pUserId IS NULL
	BEGIN
		RAISERROR('Invalid @pUserId', 16, 0);
		RETURN -2;		
	END

	SET @pSortColumn = COALESCE(@pSortColumn, 'DataOraInizioSchedules');
	-- Check colonna di ordinamento (avoid injection)
	IF LOWER(@pSortColumn) NOT IN (N'dataorainizioschedules')
	BEGIN
		RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
		RETURN -1;
	END 

	SET @pSortColumn = QUOTENAME(@pSortColumn)
	SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END
	SET @pPageSize = COALESCE(@pPageSize, 25);
	SET @pPageNumber = COALESCE(@pPageNumber, 1);

	SET @sql = N'
		SELECT adc.*,
			   cu.*,
			   T.HasAbbonamento AS CanBeconfirmedAppuntamentiDaConfermare
		FROM [dbo].[vAppuntamentiDaConfermareFull] adc
		    inner join [dbo].[vClientiUtenti] cu ON cu.UserIdClientiUtenti = adc.UserIdAppuntamentiDaConfermare 
												AND cu.IdClienteClientiUtenti = adc.IdClienteAppuntamentiDaConfermare
			inner join TipologieLezioni tl ON adc.IdTipoLezioneSchedules = tl.Id
			outer apply [dbo].[ExistsAbbonamentoValido](@pIdCliente, adc.UserIdAppuntamentiDaConfermare, tl.Livello) AS T
		WHERE adc.UserIdAppuntamentiDaConfermare = @pUserId
	'
	IF @pIdCliente IS NOT NULL
	BEGIN
		SET @sql = @sql + ' AND adc.IdClienteAppuntamentiDaConfermare = @pIdCliente ' + @newLine
	END
	IF @pScheduleStartDate IS NOT NULL
	BEGIN
		SET @sql = @sql + ' AND adc.DataOraInizioSchedules >= @pScheduleStartDate ' + @newLine
	END
	IF @pScheduleEndDate IS NOT NULL
	BEGIN
		SET @sql = @sql + ' AND adc.DataOraInizioSchedules <= @pScheduleEndDate ' + @newLine
	END
	IF COALESCE(@pIncludeDeleted, 0) <> 1
	BEGIN
		SET @sql = @sql + ' AND adc.DataCancellazioneAppuntamentiDaConfermare IS NULL ' + @newLine
	END
	IF COALESCE(@pIncludeExpired, 0)<> 1
	BEGIN
		SET @sql = @sql + ' AND adc.DataExpirationAppuntamentiDaConfermare > SYSDATETIME()' + @newLine
	END
	
	SET @sql = @sql + 'ORDER BY ' + @pSortColumn  + ' ' + @sortDirection + '
					OFFSET @pPageSize * (@pPageNumber - 1) ROWS
					FETCH NEXT @pPageSize ROWS ONLY';

	EXEC sp_executesql @sql, N'@pPageSize int, @pPageNumber int, @pIdCliente int, @pUserId VARCHAR(100), @pScheduleStartDate DATETIME2(2), @pScheduleEndDate DATETIME2(2)', 
						@pPageSize=@pPageSize, @pPageNumber=@pPageNumber, @pIdCliente=@pIdCliente, @pUserId=@pUserId, @pScheduleStartDate=@pScheduleStartDate, @pScheduleEndDate=@pScheduleEndDate
	
END