/*
La procedura consente la modifica dei dati di uno Schedule (o di più di uno se lo Schedule è ricorrente).
## Una volta creato, non è più possibile cambiare la tipologia di lezione associata allo schedule. Eventualmente dovrà essere eliminato e ricreato.
## Al variare di alcuni campi di interesse per l'utente, è necessario generare un evento di notifica (se abilitata la notifica per il cliente). Questi campi sono:
   - DataOraInizio: se cambia la data dello Schedule è necessario notificarlo all'utente
   - Location
 

## GESTIONE RICORRENTE ##
Distinguiamo i seguenti casi:
- Schedule singolo -> Controlliamo che i posti inseriti siano sufficienti per le prenotazioni già esistenti 
- Schedule ricorrrente + TipoModifica = 'S' -> lo schedule diventa indipendente (azzeramento IdParent), stesso controllo di cui sopra
- Schedule ricorrrente + TipoModifica = 'N' -> 

Gestione modifica RICORRENZA:
- Consentiamo la modifica della ricorrenza SOLO se non ci sono prenotazioni NE PER LO SCHEDULE CORRENTE NE PER TUTTI QUELLI FUTURI
- Per comodità cancelliamo tutti gli eventi e li ricreiamo
- Se cambia la DataFine:
	#se viene ridotta => cancelliamo gli eventi successivi
	#se viene aumentata => aggiungiamo gli eventi successivi
- Se cambia il tipo di ricorrenza => cancelliamo i vecchi eventi e creiamo i nuovi
- Se cambiano i DaysOfWeek => cancelliamo quelli non più presenti e creiamo i nuovi

- Può essere modificata una volta inserita?
- Se si, solo sul primo evento e su uno qualsiasi?
- Che succede se esistono delle prenotazioni e la modifica cambia i "giorni" la prenotazione
*/
CREATE PROCEDURE [dbo].[Schedules_Update]
	@pId					INT,
	@pIdCliente				INT,
	@pTitle					NVARCHAR(100),
	--@pIdTipoLezione			INT,
	@pIdLocation			INT,
	@pDataOraInizio			DATETIME2(2),
	@pIstruttore			NVARCHAR(150) = NULL,
	@pPosti					INT,
	@pCancellazionePossib	BIT,	
	@pCancellabileFinoAl	DATETIME2(2) = NULL,
	@pDataAperturaIscriz	DATETIME2(2) = NULL,
	@pDataChiusuraIscriz	DATETIME2(2) = NULL,
	@pVisibileDal			DATETIME2(2) = NULL,
	@pVisibileFinoAl		DATETIME2(2) = NULL,
	@pNote					NVARCHAR(1000) = NULL,
	@pUserIdOwner			NVARCHAR(450) = NULL,
	@pRecurrency			NVARCHAR(MAX) = NULL,
	@pWaitListDisponibile	BIT,
	@pTipoModifica			CHAR(1) = 'S' -- S = Singolo Schedule, N = Corrente + Successivi
AS
BEGIN
	DECLARE @oldRecurrency		NVARCHAR(MAX),
			@oldIdParent		INT,
			@oldDataOraInizio	DATETIME2(2),
			@oldIdLocation		INT,
			@oldIdTipoLezione	INT,
			@numPrenotazioni	INT,
			@recurrencyEqual	BIT,
			@differences		INT,
			@numNuoviPosti		INT,
			@waitListPossibile	BIT = 0,
			@oldPostiDispon		INT,
			@numPromotedWL		INT,
			@minuteDifference	INT


	SET @pTipoModifica = UPPER(COALESCE(@pTipoModifica, 'S'))
	IF @pTipoModifica NOT IN ('S', 'N')
	IF @pRecurrency IS NOT NULL AND ISJSON(@pRecurrency) = 0
	BEGIN
		RAISERROR(N'Il paramentro @pRecurrency non contiente un JSON valido', 16, 1);
		RETURN -5;
	END	 

BEGIN TRANSACTION
SET XACT_ABORT ON;

	-- Leggiamo i dati dello Schedule prima di aggiornarlo lockando il record 
	SELECT  @oldRecurrency = COALESCE(s.Recurrency, p.Recurrency),
			@oldIdParent = s.IdParent,
			@oldDataOraInizio = s.DataOraInizio,
			@oldPostiDispon = s.PostiDisponibili,
			@oldIdLocation = s.IdLocation,
			@oldIdTipoLezione = s.IdTipoLezione,
			@numPrenotazioni = (s.PostiDisponibili - s.PostiResidui),
			@waitListPossibile = CASE WHEN COALESCE(s.WaitListDisponibile,0) = 1 AND s.PostiResidui = 0 THEN 1 ELSE 0 END
	FROM Schedules s WITH (UPDLOCK)
		LEFT JOIN Schedules p ON s.IdParent = p.Id -- join con l'eventuale parent per prendere la recurrency
	WHERE s.Id = @pId 
	AND s.IdCliente = @pIdCliente

	-- Se ci sono prenotazioni, dobbiamo considerare che i Posti disponibili, se modificati, non possono essere inferiori alle pronotazioni già prese
	IF @pPosti < @numPrenotazioni
	BEGIN
		ROLLBACK
		RAISERROR(N'Impossibile ridurre i posti disponibile al di sotto del numero di prenotiazioni già prese', 16 ,1);
		RETURN -1;
	END

	-- In ogni caso aggiorniamo l'evento corrente che diventa un evento indipendente (se già non lo era)
	UPDATE Schedules
		SET Title = @pTitle, 
			--IdTipoLezione = @pIdTipoLezione, -- non è possibile modificare il TipoLezione di uno Schedule
			IdLocation = @pIdLocation, 
			DataOraInizio = @pDataOraInizio, 
			Istruttore = @pIstruttore, 
			PostiDisponibili = @pPosti, 
			PostiResidui = (@pPosti - @numPrenotazioni), 
			CancellabileFinoAl = @pCancellabileFinoAl, 
			DataChiusuraIscrizioni = @pDataChiusuraIscriz, 
			DataAperturaIscrizioni = @pDataAperturaIscriz,
			Note = @pNote, 
			UserIdOwner = @pUserIdOwner,
			Recurrency = CASE WHEN @oldIdParent IS NULL OR @pTipoModifica = 'S' THEN NULL -- Se è una modifica ad un sinogolo evento o ad un evento indipendente, non può avere una ricorrenza
								WHEN @oldIdParent IS NOT NULL AND @pTipoModifica = 'N' THEN @pRecurrency
							END,
			CancellazioneConsentita = @pCancellazionePossib, 
			WaitListDisponibile = @pWaitListDisponibile,
			VisibileDal = @pVisibileDal,
			VisibileFinoAl = @pVisibileFinoAl,
			IdParent = NULL -- Diventa un evento autonomo a prescindere dal tipo di modifica
	WHERE Id = @pId
	AND IdCliente = @pIdCliente

	-- Se sono aumentati i posti e c'erano utenti in Waiting list, devono essere convertiti
	IF (@pPosti > @oldPostiDispon ) AND (@waitListPossibile = 1)
	BEGIN
		SET @numNuoviPosti = @pPosti - @oldPostiDispon;
		EXEC [dbo].[internal_ListeAttesa_PromuoviToAppuntamento] @numNuoviPosti, @pId, @numPromotedWL OUT
	END

	--Se è stata cambiata la DataOraInizio dell'evento dobbiamo notificarlo agli utenti (sempre che la funzionalità sia abilitata a livello di Cliente)

	-- 1. generariamo degli eventi di notifica per gli utenti già iscritti
	-- 2. se ci sono utenti in WaitList aggiorniamo la scadenza dell'iscrizione
	IF @oldDataOraInizio <> @pDataOraInizio
	BEGIN
			
		-- Se è possibile che ci siano utenti in WaitingList, andiamo ad aggiornarne la scadenza
		-- La aggiorniamo per un numero di minuti 
		IF @waitListPossibile = 1
		BEGIN
			SELECT @minuteDifference = DATEDIFF(minute, @oldDataOraInizio, @pDataOraInizio)
			UPDATE ListeAttesa
				SET DataScadenza = CASE WHEN DATEADD(minute, @minuteDifference, DataScadenza) < @pDataOraInizio 
											THEN DATEADD(minute, @minuteDifference, DataScadenza) 
										ELSE @pDataOraInizio 
									END
			WHERE IdSchedule = @pId
			AND DataCancellazione IS NULL
			AND DataConversione IS NULL
		END
	END
		
	--Generiamo (eventualmente) le notifiche 
	EXEC [dbo].[internal_Schedule_NotifyChanges] @pIdCliente, @pId, @oldDataOraInizio, @pDataOraInizio, @oldIdLocation, @pIdLocation

	IF @pTipoModifica = 'N' -- Modifica a tutta la catena di eventi ==> può variare anche la ricorrenza
	BEGIN
		-- Verifichiamo che non esistano prenotazioni per l'evento corrente e per i successivi
		IF EXISTS(SELECT * FROM Appuntamenti a 
						INNER JOIN Schedules s ON A.ScheduleId = s.Id
					WHERE A.IdCliente = @pIdCliente 
					AND s.DataOraInizio >= @oldDataOraInizio -- Controlliamo solo gli eventi futuri
					AND ((s.Id = @pId) OR (S.IdParent = @pId)))
		BEGIN
			ROLLBACK
			RAISERROR('Impossibile modificare una sequenza di eventi correlati per cui esistono già prenotazioni', 16 ,1);
			RETURN -10;
		END
		-- Cancelliamo gli eventi futuri (non il corrente)
		DELETE Schedules 
		WHERE IdCliente = @pIdCliente
		AND DataOraInizio > @oldDataOraInizio
		AND (IdParent = @pId)
		-- e li reinseriamo con il corrente che diventa il padre
		EXEC [internal_Schedules_AddRicorrenti] @pId, @pIdCliente, @pTitle, @oldIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pCancellazionePossib,
														@pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pNote, @pUserIdOwner, @pRecurrency, @pWaitListDisponibile,
														@pVisibileDal, @pVisibileFinoAl
	END
COMMIT
END