/*
La procedura verifica se l'url specificato è utilizzabile o meno.
Parametri:
	@pUrlRoute: route da verificare
	@pIdCliente: se specificato si esclude la route del cliente dalla verifica
	@pResult: l'esito della verifica secondo la convenzione:
		 1: OK
		-1: UrlRoute riservato
		-2: UrlRoute già utilizzato
*/
CREATE PROCEDURE [dbo].[Clienti_RouteValidate]
	@pUrlRoute		VARCHAR(200),
	@pIdCliente		INT = NULL,
	@pResult		INT OUTPUT
AS
BEGIN
	SET @pResult = 1;
	IF EXISTS(SELECT 1 FROM ReservedRoutes WHERE UrlRoute = @pUrlRoute)
	BEGIN
		SET @pResult = -1;
	END

	IF EXISTS(SELECT 1 FROM Clienti WHERE UrlRoute = @pUrlRoute 
						AND ((@pIdCliente IS NULL) OR (Id <> @pIdCliente)))
	BEGIN
		SET @pResult = -2;
	END
END