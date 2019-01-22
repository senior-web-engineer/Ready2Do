CREATE FUNCTION [dbo].[internal_AppuntamentoDaConfermare_AsJSON]
(	@pIdAppuntamentoDaConfermare INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	RETURN(
		SELECT Id, 
			   IdCliente, 
			   UserId, 
			   ScheduleId, 
			   DataCreazione, 
			   DataExpiration,
			   DataEsito,
			   IdAppuntamento,
			   MotivoRifiuto,
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
					WHERE cu.UserId = adc.UserId AND cu.IdCliente = adc.IdCliente 
				AND cu.DataCancellazione is null
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)  AS UtenteCliente
		FROM AppuntamentiDaConfermare adc
		WHERE Id = @pIdAppuntamentoDaConfermare
		FOR JSON AUTO
	)
END
