CREATE PROCEDURE [dbo].[Clienti_Utenti_AbbonamentoSave]
	@pIdCliente				int,
	@pUserId				varchar(100),
	@pIdTipoAbbonamento		int,
	@pDataInizioValidita	datetime2(2),
	@pScadenza				datetime2(2),
	@pIngressiIniziali		smallint,
	@pIngressiResidui		smallint,
	@pImporto				decimal(10,2),
	@pImportoPagato			decimal(10,2),
	@pIdAbbonamento			int output --Se valorizzato in INPUT verrà fatto un update
AS
BEGIN
	DECLARE @deltaIngressi INT
	DECLARE @dtOperazione datetime2 = sysdatetime()

	IF @pIdAbbonamento IS NULL OR @pIdAbbonamento <= 0
	BEGIN
		INSERT INTO AbbonamentiUtenti(IdCliente, UserId, IdTipoAbbonamento, DataInizioValidita, Scadenza, IngressiIniziali, IngressiResidui, Importo, ImportoPagato)
			VALUES(@pIdCliente, @pUserId, @pIdTipoAbbonamento, @pDataInizioValidita, @pScadenza, @pIngressiIniziali, COALESCE(@pIngressiResidui, @pIngressiIniziali), @pImporto, @pImportoPagato)
		SET @pIdAbbonamento = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		DECLARE @tblChanges TABLE(IdAbbonamento INT, OldIngressiResidui SMALLINT, NewIngressiResidui SMALLINT)

		UPDATE AbbonamentiUtenti
		SET DataInizioValidita = @pDataInizioValidita,
			Scadenza = @pScadenza,
			IngressiIniziali = @pIngressiIniziali,
			IngressiResidui = COALESCE(@pIngressiResidui, @pIngressiIniziali),
			Importo = @pImporto,
			ImportoPagato = @pImportoPagato
			OUTPUT inserted.Id, deleted.IngressiResidui, inserted.IngressiResidui 
				INTO @tblChanges(IdAbbonamento, OldIngressiResidui, NewIngressiResidui)
		WHERE Id = @pIdAbbonamento
		AND IdCliente = @pIdCliente
		AND UserId = @pUserId
		AND DataCancellazione IS NULL

		IF @@ROWCOUNT = 0
		BEGIN
			RAISERROR(N'Parametri non validi', 16, 0);
			RETURN -1
		END

		--Tracciamo la modifica agli ingressi
		SELECT @deltaIngressi = NewIngressiResidui - OldIngressiResidui FROM @tblChanges
		EXEC [dbo].[internal_AbbonamentiUtenti_LogTransazione] @pIdAbbonamento, 'EDT', @deltaIngressi, @dtOperazione, NULL, NULL

	END
	RETURN 1;
END