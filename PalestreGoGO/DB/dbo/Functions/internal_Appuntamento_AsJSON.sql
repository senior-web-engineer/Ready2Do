CREATE FUNCTION [dbo].[internal_Appuntamento_AsJSON]
(	@pIdAppuntamento INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	RETURN(
		SELECT	Id,
				IdCliente,
				UserId,
				ScheduleId,
				IdAbbonamento,
				DataPrenotazione,
				DataCancellazione,
				Note,
				Nominativo
		FROM Appuntamenti
		WHERE Id = @pIdAppuntamento
		FOR JSON AUTO
	)
END
