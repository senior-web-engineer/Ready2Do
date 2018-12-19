CREATE PROCEDURE [dbo].[Appuntamenti_Lista4Utente]
	@pUserId				varchar(100),
	@pIncludeConfermati		bit,
	@pIncludeNonConfermati	bit,
	@pIncludeDeleted		bit = 0,
	@pDataInizioSched		datetime2(2) = NULL,
	@pDataFineSched			datetime2(2) = NULL,
	@pPageSize				INT = 25,
	@pPageNumber			INT = 1
AS
BEGIN
	-- nel primo recordset ritorniamo gli appuntamenti confermati (se richiesti)
	IF COALESCE(@pIncludeConfermati,0) = 1
	BEGIN
		SELECT a.* 
		FROM Appuntamenti a
			INNER JOIN Schedules s ON s.Id = a.ScheduleId
		WHERE a.UserId = @pUserId
		AND ((@pDataInizioSched IS NULL) OR (s.DataOraInizio >= @pDataInizioSched))
		AND ((@pDataFineSched IS NULL) OR (s.DataOraInizio <= @pDataFineSched))
		AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (a.DataCancellazione IS NULL))
		ORDER BY s.DataOraInizio
		OFFSET @pPageSize * (@pPageNumber -1) ROWS
		FETCH NEXT @pPageSize ROWS ONLY
	END

	IF COALESCE(@pIncludeConfermati,0) = 1
	BEGIN
		SELECT a.* 
		FROM AppuntamentiDaConfermare a
			INNER JOIN Schedules s ON s.Id = a.ScheduleId
		WHERE a.UserId = @pUserId
		AND ((@pDataInizioSched IS NULL) OR (s.DataOraInizio >= @pDataInizioSched))
		AND ((@pDataFineSched IS NULL) OR (s.DataOraInizio <= @pDataFineSched))
		AND ((COALESCE(@pIncludeDeleted, 0) = 1) OR (a.DataCancellazione IS NULL))
		ORDER BY s.DataOraInizio
		OFFSET @pPageSize * (@pPageNumber -1) ROWS
		FETCH NEXT @pPageSize ROWS ONLY
	END

END
