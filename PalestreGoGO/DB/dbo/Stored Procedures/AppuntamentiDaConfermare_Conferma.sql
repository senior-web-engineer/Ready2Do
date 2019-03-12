CREATE PROCEDURE [dbo].[AppuntamentiDaConfermare_Conferma]
	@pIdCliente					int,
	@pIdSchedule				int,
	@pIdAppuntamentoDaConf		int,
	@pIdAppuntamento			int out --Id Appuntamento confermato
AS
BEGIN
	DECLARE @MAX_LIV_CORSI SMALLINT = 32767;

	DECLARE @userId				VARCHAR(100),
			@expiration			DATETIME2,
			@idAbbonamento		INT,
			@livelloLezione		SMALLINT,
			@postiResidui		INT,
			@ingressiResidui	INT,
			@dataOp				DATETIME2 = SYSDATETIME()
			
	BEGIN TRANSACTION
	SET XACT_ABORT ON;

		SELECT @userId = UserId,
			   @expiration = adc.DataExpiration,
			   @livelloLezione = tl.Livello,
			   @postiResidui = s.PostiResidui
		FROM AppuntamentiDaConfermare adc WITH (UPDLOCK)
			INNER JOIN Schedules s WITH (UPDLOCK) ON s.Id = adc.ScheduleId
			INNER JOIN TipologieLezioni tl WITH(NOLOCK) ON tl.Id = s.IdTipoLezione
		WHERE adc.Id = @pIdAppuntamentoDaConf
		AND adc.IdCliente = @pIdCliente
		AND adc.ScheduleId = @pIdSchedule

		IF @userId IS NULL
		BEGIN
			ROLLBACK;
			RAISERROR('Impossibile confermare l''appuntamento. Parametri non validi', 16, 0);
			RETURN -1
		END

		IF @expiration < SYSDATETIME
		BEGIN
			ROLLBACK;
			RAISERROR('Impossibile confermare l''appuntamento. Appuntamento scaduto.', 16, 0);
			RETURN -2
		END

		IF @postiResidui <= 0
		BEGIN
			ROLLBACK;
			RAISERROR('Impossibile confermare l''appuntamento. Nessun posto disponibile.', 16, 0);
			RETURN -3
		END

		-- Recuperiamo l'abbonamento su cui addebitare l'appuntamento		
		SELECT TOP 1 
				@idAbbonamento = au.Id,
				@ingressiResidui = au.IngressiResidui
		FROM AbbonamentiUtenti au WITH (UPDLOCK)
			INNER JOIN TipologieAbbonamenti ta WITH (NOLOCK) ON au.IdTipoAbbonamento = ta.Id
		WHERE au.DataInizioValidita <= SYSDATETIME() -- nel periodo di validità
		AND au.Scadenza < SYSDATETIME()
		AND COALESCE(au.IngressiResidui, 1) > 0 -- con almeno un ingresso residuo se previsti
		AND au.DataCancellazione IS NULL --non cancellato
		AND au.IdCliente = @pIdCliente
		AND au.UserId = @userId
		AND COALESCE(ta.MaxLivCorsi, @MAX_LIV_CORSI) >= COALESCE(@livelloLezione, 0) -- livello corsi compatibile
		ORDER BY au.DataInizioValidita ASC

		IF @idAbbonamento IS NULL
		BEGIN
			ROLLBACK;
			RAISERROR('Impossibile confermare l''appuntamento. Nessun abbonamento su cui addebitare la lezione trovato.', 16, 0);
			RETURN -4
		END
		
		-- Decrementiamo i posti disponibili per l'evento (schedule)
		UPDATE Schedules
			SET PostiResidui = PostiResidui -1
		WHERE Id = @pIdSchedule
		AND PostiResidui > 0
		AND DataOraInizio > @dataOp -- solo per Eventi non ancora iniziati

		IF @@ROWCOUNT <> 1
		BEGIN
			ROLLBACK;
			RAISERROR('Impossibile confermare l''appuntamento. Nessun posto disponibile o evento già iniziato.', 16, 0);
			RETURN -5
		END

		-- Inseriamo l'appuntamento
		INSERT INTO Appuntamenti (IdCliente, UserId, ScheduleId, IdAbbonamento, DataPrenotazione, Note, Nominativo)
			VALUES(@pIdCliente, @userId, @pIdSchedule, @idAbbonamento, @dataOp, 'Conferma appuntamento', NULL)
		SET @pIdAppuntamento = SCOPE_IDENTITY();

		-- Se è un abbonamento ad ingressi, decrementiamo il numero di ingressi residui
		IF @ingressiResidui IS NOT NULL
		BEGIN
			UPDATE AbbonamentiUtenti SET IngressiResidui = IngressiResidui -1 WHERE Id = @idAbbonamento
			EXEC [dbo].[internal_AbbonamentiUtenti_LogTransazione] @idAbbonamento, 'APP', -1, @dataOp, @pIdAppuntamento, NULL
		END

	COMMIT
END