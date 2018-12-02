/*
@pRecurrency -> JSON nel formato:
	{
	 "Recurrency" : "Weekly", 
	 "DaysOfWeek": ["Lun", "Mar", "Ven"],
	 "RepeatUntil": "YYYY-MM-DD"
	}
DOVE:
"Recurrency" può assumere uno dei seguenti valori:
	- Daily
	- Weekly
	- Monthly
"DaysOfWeek" ha senso solo se Recurrency = Weekly con la seguente semantica:
	- Se Recurrency == Weekly AND DaysOfWeek null (o non presente) ==> l'evento si ripete una sola volta a settimana lo stesso giorno fino alla fine (Es: tutti i martedì)
	- Se Recurrency == Weekly AND DaysOfWeek specificato ==> l'evento si ripete tutte le settimane nei giorni indicati
"RepeatUntil" deve essere presente ed indica fino a quando si ripetono gli eventi e si intende INCLUSIVA (MAX + 2ANNI)
*/
CREATE PROCEDURE [dbo].[Schedule_Add]
	@pIdCliente				INT = 0,
	@pTitle					NVARCHAR(100),
	@pIdTipoLezione			INT,
	@pIdLocation			INT,
	@pDataOraInizio			DATETIME2(2),
	@pIstruttore			NVARCHAR(150) = NULL,
	@pPosti					INT,
	@pCancellabileFinoAl	DATETIME2(2),
	@pDataAperturaIscriz	DATETIME2(2),
	@pDataChiusuraIscriz	DATETIME2(2),
	@pNote					NVARCHAR(1000) = NULL,
	@pUserIdOwner			NVARCHAR(450) = NULL,
	@pRecurrency			NVARCHAR(MAX) = NULL,
	@pId					INT OUTPUT
AS
BEGIN
SET XACT_ABORT ON;
	--DECLARE @tblSchedules TABLE (
	--			[IdCliente]					INT				NOT NULL,
	--			[Title]						NVARCHAR(100)	NOT NULL,
	--			[IdTipoLezione]				INT				NOT NULL,
	--			[IdLocation]				INT				NOT NULL,
	--			[DataOraInizio]				DATETIME2(2)	NOT NULL,
	--			[Istruttore]				NVARCHAR(150)	NULL,
	--			[PostiDisponibili]			INT				NOT NULL,	-- Posti inizilamente disponibili
	--			[PostiResidui]				INT				NOT NULL,	-- Contatore dei posti ancora disponibili
	--			[CancellabileFinoAl]		DATETIME2(2)	NOT NULL,
	--			[DataAperturaIscrizioni]	DATETIME2(2)	NOT NULL,
	--			[DataChiusuraIscrizioni]	DATETIME2(2)	NOT NULL,
	--			[DataCancellazione]			DATETIME2(2)	NULL,		-- Valorizzata se la classe è stata cancellata dalla palestra
	--			[UserIdOwner]				NVARCHAR(450)	NULL,	-- Utente titolare della classe (se valorizzato è l'unico che può editare la classe)
	--			[Note]						NVARCHAR(1000)	NULL,
	--			[Recurrency]				NVARCHAR(MAX)	NULL,
	--			[IdParent]					INT				NULL
	--		)
	
	IF @pId IS NOT NULL AND @pId > 0
	BEGIN
		RAISERROR(N'Questa procedura non può modificare Schedule esistenti', 16, 1);
		RETURN -4;
	END

	-- EVENTO RICORRENTE
	IF @pRecurrency IS NOT NULL
	BEGIN 
		IF ISJSON(@pRecurrency) = 0
		BEGIN
			RAISERROR(N'Il paramentro @pRecurrency non contiente un JSON valido', 16, 1);
			RETURN -5;
		END	
 	--	SELECT  @recurrency = LOWER(JSON_VALUE(@pRecurrency, '$.Recurrency')),
		--		@endData = CAST(JSON_VALUE(@pRecurrency, '$.RepeatUntil') AS DATE),
		--		@firstDay = JSON_VALUE(@pRecurrency, '$.DaysOfWeek[0]')
		---- Verifica parametri
		--IF @recurrency NOT IN ('daily','weekly', 'monthly')
		--BEGIN
		--	RAISERROR(N'Il paramentro @pRecurrency contiene una frequenza non supportata', 16, 1);
		--	RETURN -6;
		--END
		--IF @endData IS NULL OR @endData > DATEADD(year, 2, GETDATE()) OR @endData < @pDataOraInizio
		--BEGIN
		--	RAISERROR(N'Il paramentro @pRecurrency contiene una data fine non valida', 16, 1);
		--	RETURN -7;
		--END
		BEGIN TRANSACTION
		-- Inseriamo l'evento padre
		INSERT INTO Schedules(IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, [Recurrency])
			VALUES( @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pPosti, @pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz,@pNote, @pUserIdOwner, @pRecurrency)
		SET @pId = SCOPE_IDENTITY();

		EXEC [internal_Schedules_AddRicorrenti] @pId, @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, 
														@pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz, @pNote, @pUserIdOwner, @pRecurrency		

		---- Generazioni eventi figli
		--IF @recurrency = 'daily'
		--BEGIN
		--	INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent)
		--		SELECT @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, 							
		--				DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME)),
		--				@pIstruttore, @pPosti, @pPosti,
		--				-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pCancellabileFinoAl, @pDataOraInizio),
		--							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
		--				-- Stessa cosa per DataAperturaIscrizioni
		--				DATEADD(second, 
		--							DATEDIFF(SECOND, @pDataAperturaIscriz, @pDataOraInizio),
		--							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
		--				-- Stessa cosa per DataChiusuraIscrizioni
		--				DATEADD(second, 
		--							DATEDIFF(SECOND, @pDataChiusuraIscriz, @pDataOraInizio),
		--							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
		--				@pNote, 
		--				@pUserIdOwner, 
		--				@pId
		--		FROM [utils].[Giorni] AS gi
		--		WHERE gi.Data > @pDataOraInizio
		--		AND gi.Data <= @endData
		--END
		--ELSE IF @recurrency = 'monthly'
		--BEGIN
		--	INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent)
		--		SELECT @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation,
		--				-- DataInizio = DataInizioOriginale + i-esimo mese
		--				DATEADD(MONTH, n.Number, @pDataOraInizio),
		--				@pIstruttore, @pPosti, @pPosti,
		--				-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pCancellabileFinoAl, @pDataOraInizio),
		--							DATEADD(MONTH, n.Number, @pDataOraInizio)),
		--				-- Stessa cosa per DataAperturaIscrizioni
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pDataAperturaIscriz, @pDataOraInizio),
		--							DATEADD(MONTH, n.Number, @pDataOraInizio)),
		--				-- Stessa cosa per DataChiusuraIscrizioni
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pDataChiusuraIscriz, @pDataOraInizio),
		--							DATEADD(MONTH, n.Number, @pDataOraInizio)),
		--				@pNote, 
		--				@pUserIdOwner, 
		--				@pId
		--	FROM [utils].[Numbers] AS n
		--	WHERE n.Number > 0
		--	AND n.Number < DATEDIFF(MONTH, @pDataOraInizio, @endData)				
		--END
		--ELSE IF @recurrency = 'weekly'
		--BEGIN				
		--	--Caso in cui non ci sono DaysOfWeek specificati ==> vuol dire tutte le settimane, lo stesso giorno dell'evento parent (Es: tutti i martedì)
		--	IF @firstDay IS NULL
		--	BEGIN
		--		INSERT INTO @tblWeekDays (WeekDayNum)
		--			SELECT DATEPART(dw, @pDataOraInizio)
		--	END
		--	ELSE
		--	-- Se invece sono specificati dei DaysOfWeek  => vuol dire tutte le settimane, più giorni la settimana (Es: lun, mer, ven)
		--	BEGIN
		--		INSERT INTO @tblWeekDays (WeekDayNum)
		--		select gi.DayOfWeekNum
		--			FROM (
		--				SELECT lower([DayOfWeek]) AS [DayOfWeek]
		--				FROM OPENJSON( @pRecurrency, '$.DaysOfWeek' ) 
		--				WITH ([DayOfWeek] NVARCHAR(25) '$')) rg
		--			-- Prendiamo arbitrariamente la seconda settimana del 2018 per recuperare il DayOfWeekNum
		--			INNER JOIN [utils].[Giorni] gi ON gi.WeekNum = 2 AND lower(gi.DayOfWeekItaShort) = rg.[DayOfWeek] AND gi.Year = 2018
		--	END
		--	INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent)
		--		SELECT @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, 							
		--				DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME)),
		--				@pIstruttore, @pPosti, @pPosti,
		--				-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pCancellabileFinoAl, @pDataOraInizio),
		--							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
		--				-- Stessa cosa per DataAperturaIscrizioni
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pDataAperturaIscriz, @pDataOraInizio),
		--							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
		--				-- Stessa cosa per DataChiusuraIscrizioni
		--				DATEADD(SECOND, 
		--							DATEDIFF(SECOND, @pDataChiusuraIscriz, @pDataOraInizio),
		--							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
		--				@pNote, 
		--				@pUserIdOwner, 
		--				@pId
		--		FROM [utils].[Giorni] AS gi
		--			INNER JOIN @tblWeekDays tdw ON tdw.WeekDayNum = gi.DayOfWeekNum
		--		WHERE gi.Data > @pDataOraInizio
		--		AND gi.Data <= @endData
		--END
		COMMIT
	END
	ELSE
	-- EVENTO SINGOLO (NON RICORRENTE)
	BEGIN
		INSERT INTO Schedules(IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner)
			VALUES( @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, @pDataOraInizio, @pIstruttore, @pPosti, @pPosti, @pCancellabileFinoAl, @pDataAperturaIscriz, @pDataChiusuraIscriz,@pNote, @pUserIdOwner)

		SET @pId = SCOPE_IDENTITY();
	END
	RETURN 1;
END