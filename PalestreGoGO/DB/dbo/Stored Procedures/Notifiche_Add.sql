CREATE PROCEDURE [dbo].[Notifiche_Add]
	@pIdTipo				INT,
	@pUserId				VARCHAR(100),
	@pIdCliente				INT = NULL,
	@pTitolo				NVARCHAR(50),
	@pTesto					NVARCHAR(250),
	@pActionUrl				VARCHAR(5000),
	@pDataInizioVisibilita	DATETIME2 = NULL,
	@pDataFineVisibilita	DATETIME2 = NULL,
	@pIdNotitifica			BIGINT OUTPUT
AS
BEGIN
	DECLARE @autodismissAfter	BIGINT,
			@dtEndVis			DATETIME2
	IF (@pDataInizioVisibilita IS NULL)
	BEGIN
		SELECT @autodismissAfter = tn.AutoDismissAfter 
			FROM TipologieNotifiche tn WHERE Id = @pIdTipo;
		IF @autodismissAfter IS NOT NULL
		BEGIN
			SET @pDataFineVisibilita = DATEADD(SECOND, @autodismissAfter, SYSDATETIME())
		END
	END
	ELSE
	BEGIN
		SET @dtEndVis = @pDataFineVisibilita;
	END
	INSERT INTO Notifiche(IdTipo, UserId, IdCliente, Titolo, Testo, DataInizioVisibilita, DataFineVisibilita, ActionUrl)
		VALUES (@pIdTipo, @pUserId, @pIdCliente, @pTitolo, @pTesto, @pDataInizioVisibilita, @dtEndVis, @pActionUrl);

	SET @pIdNotitifica = SCOPE_IDENTITY();
END