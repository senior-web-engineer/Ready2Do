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
			   (SELECT * FROM ClientiUtenti cu 
					WHERE cu.UserId = adc.UserId AND cu.IdCliente = adc.IdCliente 
				AND cu.DataCancellazione is null
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) 
		FROM AppuntamentiDaConfermare adc
		WHERE Id = @pIdAppuntamentoDaConfermare
		FOR JSON AUTO
	)
END
