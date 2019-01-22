CREATE PROCEDURE [dbo].[Appuntamenti_Lista4Schedule]
	@pIdCliente				INT,
	@pIdSchedule			INT,
	@pIncludeDeleted		BIT = 0
AS
BEGIN
	SELECT    a.*,
			 cu.*
	FROM vAppuntamenti a
		LEFT JOIN vClientiUtenti cu ON cu.IdClienteClientiUtenti = a.IdClienteAppuntamenti AND cu.UserIdClientiUtenti = a.UserIdAppuntamenti AND cu.DataCancellazioneClientiUtenti is null
	WHERE a.IdClienteAppuntamenti = @pIdCliente
		AND a.ScheduleIdAppuntamenti = @pIdSchedule
		AND ((COALESCE(@pIncludeDeleted,0) = 1) OR (a.DataCancellazioneAppuntamenti IS NULL))
END