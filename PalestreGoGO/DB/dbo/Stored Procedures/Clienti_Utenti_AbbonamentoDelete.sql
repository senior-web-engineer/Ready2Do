CREATE PROCEDURE [dbo].[Clienti_Utenti_AbbonamentoDelete]
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pIdAbbonamento		int 
AS
BEGIN
	-- Come ci comportiamo con le lezioni eventualmente prenotate se cancelliamo un abbonamento ?
	-- Per ora ci limitiamo ad impedire la cancellazione se ne esistotno
	IF EXISTS(SELECT * FROM Appuntamenti a with(nolock)
				INNER JOIN Schedules s with (nolock) ON a.ScheduleId = s.Id
				WHERE a.IdCliente = @pIdCliente 
				AND a.UserId = @pUserId 
				AND a.IdAbbonamento = @pIdAbbonamento 
				AND s.DataOraInizio >= SYSDATETIME())
	BEGIN
		RAISERROR(N'Impossibile cancellare un abbonamento per cui esistono delle prenotazioni future', 16,0);
		RETURN -1;
	END

	UPDATE Appuntamenti
		SET DataCancellazione = SYSDATETIME()
	WHERE IdCliente = @pIdCliente 
	AND UserId = @pUserId 
	AND IdAbbonamento = @pIdAbbonamento 
	--Rafforziamo il controllo per essere sicuri che non sia stata creato un appuntamento nel frattempo
	AND NOT EXISTS(SELECT * FROM Appuntamenti a 
				INNER JOIN Schedules s ON a.ScheduleId = s.Id
				WHERE a.IdCliente = @pIdCliente 
				AND a.UserId = @pUserId 
				AND a.IdAbbonamento = @pIdAbbonamento 
				AND s.DataOraInizio >= SYSDATETIME())

	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR(N'Parametri errati', 16, 0);
		RETURN -2;
	END

	RETURN 1;
END