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
				Nominativo,
			   (SELECT cu.IdCliente AS IdCliente,
					   cu.Cognome AS Cognome,
					   cu.DataAggiornamento AS DataAggiornamento,
					   cu.DataCancellazione AS DataCancellazione,
					   cu.DataCreazione AS DataCreazione,
					   cu.Nome AS Nome,
					   cu.UserDisplayName AS UserDisplayName,
					   cu.UserId AS UserId
				FROM ClientiUtenti cu 
				WHERE cu.UserId = app.UserId AND cu.IdCliente = app.IdCliente 
				AND cu.DataCancellazione IS NULL
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS UtenteCliente
		FROM Appuntamenti app
		WHERE Id = @pIdAppuntamento
		FOR JSON AUTO
	)
END