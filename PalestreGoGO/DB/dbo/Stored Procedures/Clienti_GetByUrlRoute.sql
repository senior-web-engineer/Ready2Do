/*
Ritorna i dati di un Cliente partendo dalla Route URL
*/
CREATE PROCEDURE [dbo].[Clienti_GetByUrlRoute]
	@pUrlRoute			VARCHAR(205)
AS
BEGIN

	SELECT c.*
	FROM [vClientiProvisioned] c
	WHERE UrlRoute = @pUrlRoute
END