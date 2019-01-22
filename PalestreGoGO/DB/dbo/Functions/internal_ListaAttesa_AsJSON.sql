CREATE FUNCTION [dbo].internal_ListaAttesa_AsJSON
(	@pIdListaAttesa INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	RETURN(
		SELECT	Id,
				IdCliente,
				UserId,
				IdSchedule,
				IdAbbonamento,
				DataCreazione,
				DataCancellazione,
				DataScadenza,
				DataConversione,
				DataCancellazione,
				(SELECT cu.IdCliente AS IdCliente,
					   cu.Cognome AS Cognome,
					   cu.DataAggiornamento AS DataAggiornamento,
					   cu.DataCancellazione AS DataCancellazione,
					   cu.DataCreazione AS DataCreazione,
					   cu.Nome AS Nome,
					   cu.UserDisplayName AS UserDisplayName,
					   cu.UserId AS UserId
				FROM ClientiUtenti cu 
					WHERE cu.UserId = wl.UserId AND cu.IdCliente = wl.IdCliente 
					AND cu.DataCancellazione is null
					FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS UtenteCliente
		FROM [ListeAttesa] wl
		WHERE Id = @pIdListaAttesa
		FOR JSON AUTO
	)
END
