/*
@pRecurrency -> JSON nel formato:
	{
	 "Recurrency" : "Weekly", 
	 "DaysOfWeek": ["Lun", "Mar", "Ven"],
	 "RepeatUntil": "YYYY-MM-DD",
	 "RepeatFor": "xxx"
	}
DOVE:
"Recurrency" può assumere uno dei seguenti valori:
	- Daily
	- Weekly
	- Monthly
"DaysOfWeek" ha senso solo se Recurrency = Weekly con la seguente semantica:
	- Se Recurrency == Weekly AND DaysOfWeek null (o non presente) ==> l'evento si ripete una sola volta a settimana lo stesso giorno fino alla fine (Es: tutti i martedì)
	- Se Recurrency == Weekly AND DaysOfWeek specificato ==> l'evento si ripete tutte le settimane nei giorni indicati
"RepeatUntil" | "RepeatFor": SOLO uno dei deve essere necessariamente presente ed indica fino a quando si ripetono gli eventi e si intende INCLUSIVA (MAX + 2ANNI) nel caso di RepeatUntil
							 nel caso di RepeatFor il Max e 500 ed il primo evento si conta come 1 per cui RepatFor=2 genera l'evento principale ed un evento figlio
*/
CREATE PROCEDURE [dbo].[Schedules_Add]
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
	@pVisibileDal			DATETIME2(2) = NULL,
	@pVisibileFinoAl		DATETIME2(2) = NULL,
	@pNote					NVARCHAR(1000) = NULL,
	@pUserIdOwner			NVARCHAR(450) = NULL,
	@pRecurrency			NVARCHAR(MAX) = NULL,
	@pWaitListDisponibile	BIT,
	@pId					INT OUTPUT
AS
BEGIN
SET XACT_ABORT ON;
	DECLARE @logMsg	NVARCHAR(2000)
	IF @pId IS NOT NULL AND @pId > 0
	BEGIN
		RAISERROR(N'Questa procedura non può modificare Schedule esistenti', 16, 1);
		RETURN -4;
	END

	-- EVENTO RICORRENTE
	IF @pRecurrency IS NOT NULL
	BEGIN 
		SET @logMsg = CONCAT('Inserimento nuovo evento ricorrente. Title:', @pTitle, ' - IdTipoLezione:', @pIdTipoLezione, ', IdLocation:', @pRecurrency,', Recurrency:', @pRecurrency)
		exec internal_LogMessage @pIdCliente, NULL, 'V', @logMsg
		IF ISJSON(@pRecurrency) = 0
		BEGIN
			RAISERROR(N'Il paramentro @pRecurrency non contiente un JSON valido', 16, 1);
			RETURN -5;
		END	
 	
		BEGIN TRANSACTION
		-- Inseriamo l'evento padre
		INSERT INTO Schedules(IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellazioneConsentita, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, VisibileDal, VisibileFinoAl, Note, UserIdOwner, [Recurrency], WaitListDisponibile)
			VALUES( @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pPosti, @pCancellazionePossib, @pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pVisibileDal, @pVisibileFinoAl, @pNote, @pUserIdOwner, @pRecurrency, @pWaitListDisponibile)
		SET @pId = SCOPE_IDENTITY();

		SET @logMsg = CONCAT('Inserito evento Parent con Id:', @pId)
		exec internal_LogMessage @pIdCliente, NULL, 'V', @logMsg


		EXEC [internal_Schedules_AddRicorrenti] @pId, @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pCancellazionePossib, 
														@pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pNote, @pUserIdOwner,
														@pRecurrency, @pWaitListDisponibile, @pVisibileDal, @pVisibileFinoAl

		COMMIT
	END
	ELSE
	-- EVENTO SINGOLO (NON RICORRENTE)
	BEGIN
		INSERT INTO Schedules(IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellazioneConsentita, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, VisibileDal, VisibileFinoAl, Note, UserIdOwner, WaitListDisponibile)
			VALUES( @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pPosti, @pCancellazionePossib, @pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pVisibileDal, @pVisibileFinoAl, @pNote, @pUserIdOwner, @pWaitListDisponibile)

		SET @pId = SCOPE_IDENTITY();
		SET @logMsg = CONCAT('Inserito evento Singolo con Id:', @pId)
		exec internal_LogMessage @pIdCliente, NULL, 'V', @logMsg
	END
	RETURN 1;
END
