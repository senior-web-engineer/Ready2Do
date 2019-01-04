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
				DataCancellazione
		FROM [ListeAttesa]
		WHERE Id = @pIdListaAttesa
		FOR JSON AUTO
	)
END
