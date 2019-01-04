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
			   DataCancellazione
		FROM AppuntamentiDaConfermare
		WHERE Id = @pIdAppuntamentoDaConfermare
		FOR JSON AUTO
	)
END
