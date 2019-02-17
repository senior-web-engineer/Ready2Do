/*
Ritorna i dati di un Cliente partendo dalla Route URL
*/
CREATE PROCEDURE [dbo].[Clienti_GetById]
	@pIdCliente			INT
AS
BEGIN

	SELECT c.*
	FROM [vClientiProvisioned] c
	WHERE Id = @pIdCliente
END