/*
Aggiorna l'anagrafica del Cliente
*** History ***
20190803 - Fix: l'urlRoute non veniva aggiornato
*/
CREATE PROCEDURE [dbo].[Clienti_AnagraficaSave]
	@pIdCliente			INT,
	@pNome				NVARCHAR(100),
	@pRagioneSociale	NVARCHAR(100),
	@pEmail				VARCHAR(100),
	@pNumTelefono		VARCHAR(50) = NULL,
	@pDescrizione		NVARCHAR(1000) = NULL,
	@pIndirizzo			NVARCHAR(250),
	@pCitta				NVARCHAR(100),
	@pPostalCode		NVARCHAR(10) = NULL,
	@pCountry			NVARCHAR(100) = NULL,
	@pLatitudine		FLOAT,
	@pLongitudine		FLOAT,
	@pUrlRoute			VARCHAR(205),
	@pRouteIsChanged	BIT OUTPUT
AS
BEGIN
	DECLARE @urlIsValid INT
	DECLARE @tblChanges TABLE (OldUrlRoute VARCHAR(205), NewUrlRoute VARCHAR(205))

	EXEC [dbo].[Clienti_RouteValidate] @pUrlRoute, @pIdCliente, @urlIsValid OUTPUT

	IF @urlIsValid  = -1
	BEGIN
		RAISERROR('La Route specificata risulta riservata', 16, 1);
		RETURN -1;
	END
	ELSE IF @urlIsValid  = -2
	BEGIN
		RAISERROR('La Route specificata risulta già utilizzata', 16, 1);
		RETURN -2;
	END
	ELSE IF @urlIsValid  <> 1
	BEGIN
		RAISERROR('La Route specificata risulta non risulta valida', 16, 1);
		RETURN -3;
	END

	UPDATE Clienti
	SET Nome = @pNome,
		RagioneSociale = @pRagioneSociale,
		Email = @pEmail,
		NumTelefono = @pNumTelefono,
		Descrizione = @pDescrizione,
		Indirizzo = @pIndirizzo,
		Citta = @pCitta,
		ZipOrPostalCode = @pPostalCode,
		Country = @pCountry,
		Latitudine = @pLatitudine,
		Longitudine = @pLongitudine,
		UrlRoute = @pUrlRoute
		OUTPUT deleted.UrlRoute, inserted.UrlRoute INTO @tblChanges(OldUrlRoute, NewUrlRoute)
	WHERE Id = @pIdCliente
	AND IdStato >= 3 -- Solo provisioned

	IF @@ROWCOUNT <> 1
	BEGIN
		RAISERROR(N'Impossibile trovare il Cliente [%i] da aggiornare',16,1,@pIdCliente);
		RETURN -1
	END

	IF EXISTS(SELECT * FROM @tblChanges WHERE OldUrlRoute = NewUrlRoute)
		SET @pRouteIsChanged = 0
	ELSE
		SET @pRouteIsChanged = 1

	RETURN 0;
END