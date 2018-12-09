/*
Cancella logicamente una Tipologia Lezione.
L'operazione è possibile solo se per gli eventuali scheudli esistenti (futuri) non ci sono prenotazioni.
- Se risulta anche solo una prenotazione in essere per questa tipologia di lezione dovrà essere pima annullata la prenotazione 
e solo dopo potrà essere cancellata la tipologia di lezione.
- Gli Schedule associati alla Tipologia di lezione sono cancellati in cascata
*/
CREATE PROCEDURE [dbo].[TipologieLezioni_Delete]
	@pId			INT,
	@pIdCliente		INT
AS
BEGIN
	BEGIN TRANSACTION
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
	SET XACT_ABORT ON;

	DECLARE @dtOperazione DATETIME2= SYSDATETIME();

	UPDATE TipologieLezioni
		SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	AND DataCancellazione IS NULL
	AND NOT EXISTS(SELECT * FROM [Appuntamenti] a WITH (UPDLOCK)
						INNER JOIN [Schedules] s ON a.ScheduleId = s.Id AND s.IdTipoLezione = @pId
				   WHERE a.IdCliente = @pIdCliente
				   AND a.DataCancellazione IS NULL
				   AND s.IdCliente = @pIdCliente
				   AND s.DataCancellazione IS NULL
				   AND s.DataOraInizio >= @dtOperazione
				   AND s.IdTipoLezione = @pId
				   )

	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR('Tipologia lezione non cancellabile o parametri non validi!', 16, 0);
		RETURN -1;
	END
	
	--Annulliamo tutti gli Schedule per il tipo lezione
	UPDATE Schedules
		SET DataCancellazione = SYSDATETIME()
	WHERE IdTipoLezione = @pId
	AND IdCliente = @pIdCliente
	AND DataCancellazione IS NULL
	AND DataOraInizio >= @dtOperazione

	RETURN 0;
END