/*
La procedura consente la modifica dei dati di uno Schedule (o di più di uno se lo Schedule è ricorrente).
Distinguiamo i seguenti casi:
- Schedule singolo -> niente di particolare, controlliamo solo che i posti inseriti siano sufficienti per le prenotazioni già esistenti
- Schedule ricorrrente + TipoModifica = 'S' -> lo schedule diventa indipendente (azzeramento IdParent), stesso controllo di cui opra
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
	@pIdCliente				INT = 0,
	@pTitle					NVARCHAR(100),
	@pIdTipoLezione			INT,
	@pIdLocation			INT,
	@pDataOraInizio			DATETIME2(2),
	@pIstruttore			NVARCHAR(150) = NULL,
	@pPosti					INT,
	@pCancellazionePossib	BIT,	
	@pCancellabileFinoAl	DATETIME2(2) = NULL,
	@pDataAperturaIscriz	DATETIME2(2) = NULL,
	@pDataChiusuraIscriz	DATETIME2(2) = NULL,
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
			@numPrenotazioni	INT,
			@recurrencyEqual	BIT,
			@differences		INT


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
			@numPrenotazioni = (s.PostiDisponibili - s.PostiResidui) 
	FROM Schedules s WITH (UPDLOCK)
		LEFT JOIN Schedules p ON s.IdParent = p.Id -- join con l'eventuale parent per prendere la recurrency
	WHERE s.Id = @pId 
	AND s.IdCliente = @pIdCliente

	--Se non era un evento ricorrente applichiamo direttamente la modifica
	IF @oldIdParent IS NULL
	BEGIN
		-- Dobbiamo considerare che i Posti disponibili  se modificati non possono essere inferiori alle pronotazioni già prese
		IF @pPosti < @numPrenotazioni
		BEGIN
			ROLLBACK
			RAISERROR(N'Impossibile ridurre i posti disponibile al di sotto del numero di prenotiazioni già prese', 16 ,1);
			RETURN -1;
		END

		UPDATE Schedules
			SET Title = @pTitle, 
				IdTipoLezione = @pIdTipoLezione, 
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
				CancellazioneConsentita = @pCancellazionePossib, 
				WaitListDisponibile = @pWaitListDisponibile
		WHERE Id = @pId
		AND IdCliente = @pIdCliente

		IF @pRecurrency IS NOT NULL
		BEGIN
			-- Se è stata specificata una ricorrenza, inseriamo gli eventi figli		
			EXEC [internal_Schedules_AddRicorrenti] @pId, @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pCancellazionePossib,
											 @pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pNote, @pUserIdOwner, @pRecurrency, @pWaitListDisponibile
		END
	END
	ELSE
	-- Se era un evento ricorrente dobbiamo gestire le varie casistiche
	BEGIN
		-- In ogni caso aggiorniamo l'evento corrente che diventa un evento indipendente
		UPDATE Schedules
			SET Title = @pTitle, 
				IdTipoLezione = @pIdTipoLezione, 
				IdLocation = @pIdLocation, 
				DataOraInizio = @pDataOraInizio, 
				Istruttore = @pIstruttore, 
				PostiDisponibili = @pPosti, 
				PostiResidui = (@pPosti - @numPrenotazioni), 
				CancellabileFinoAl = @pCancellabileFinoAl, 
				DataChiusuraIscrizioni = @pDataChiusuraIscriz, 
				Note = @pNote, 
				UserIdOwner = @pUserIdOwner,
				Recurrency = CASE WHEN @pTipoModifica = 'S' THEN NULL -- Se è una modifica ad un sinogolo evento non può avere una ricorrenza
								 WHEN @pTipoModifica = 'N' THEN @pRecurrency
							 END,
				CancellazioneConsentita = @pCancellazionePossib, 
				WaitListDisponibile = @pWaitListDisponibile,
				IdParent = NULL -- Diventa un evento autonomo a prescindere dal tipo di modifica
		WHERE Id = @pId
		AND IdCliente = @pIdCliente

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
			EXEC [internal_Schedules_AddRicorrenti] @pId, @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pCancellazionePossib,
															@pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pNote, @pUserIdOwner, @pRecurrency, @pWaitListDisponibile
		END
	END
COMMIT
END