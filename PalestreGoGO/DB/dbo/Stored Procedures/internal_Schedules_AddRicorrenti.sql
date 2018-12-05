/*
Procedura interna per generare gli eventi ricorrenti
*/
CREATE PROCEDURE [dbo].[internal_Schedules_AddRicorrenti]
	@pIdParent				INT,
	@pIdCliente				INT = 0,
	@pTitle					NVARCHAR(100),
	@pIdTipoLezione			INT,
	@pIdLocation			INT,
	@pDataOraInizio			DATETIME2(2),
	@pIstruttore			NVARCHAR(150) = NULL,
	@pPosti					INT,
	@pCancellazionePossib	BIT,
	@pCancellabileFinoAl	DATETIME2(2),
	@pDataAperturaIscriz	DATETIME2(2) = NULL,
	@pDataChiusuraIscriz	DATETIME2(2) = NULL,
	@pNote					NVARCHAR(1000) = NULL,
	@pUserIdOwner			NVARCHAR(450) = NULL,
	@pRecurrency			NVARCHAR(MAX) = NULL,
	@pWaitListDisponibile	BIT
AS
BEGIN
	DECLARE @CONST_MAX_YEARS			INT = 2,
			@CONST_MAX_RIPETIZ		INT = 500;

	DECLARE @recurrency VARCHAR(100),
			@endData DATE,
			@numRipetizioni INT,
			@firstDay VARCHAR(100)
	DECLARE @tblWeekDays TABLE (WeekDayNum INT);
	DECLARE @diffCancellabile	BIGINT,
			@diffAperturaIscr	BIGINT,
			@diffChiusuraIscr	BIGINT

	SELECT @diffCancellabile = DATEDIFF(second,@pDataOraInizio, @pCancellabileFinoAl);
	SELECT @diffAperturaIscr = DATEDIFF(second,@pDataOraInizio, @pDataAperturaIscriz);
	SELECT @diffChiusuraIscr = DATEDIFF(second,@pDataOraInizio, @pDataChiusuraIscriz);

	SELECT  @recurrency = LOWER(JSON_VALUE(@pRecurrency, '$.Recurrency')),
				@endData = CAST(JSON_VALUE(@pRecurrency, '$.RepeatUntil') AS DATE),
				@numRipetizioni = CAST(JSON_VALUE(@pRecurrency, '$.RepeatFor') AS INT),
				@firstDay = JSON_VALUE(@pRecurrency, '$.DaysOfWeek[0]')
		-- Verifica parametri
		IF @recurrency NOT IN ('daily','weekly', 'monthly')
		BEGIN
			RAISERROR(N'Il paramentro @pRecurrency contiene una frequenza non supportata', 16, 1);
			RETURN -6;
		END
		IF @endData IS NULL AND @numRipetizioni IS NULL
		BEGIN
			RAISERROR(N'E'' necessario specificare uno tra RepeatUntil e RepeatFor in @pRecurrency', 16, 1);
			RETURN -7;
		END
		-- Se specificata la data fine non deve essere superiore a 2 anno ed antecedente la data inizio del primo evento
		IF @endData IS NOT NULL AND (@endData > DATEADD(year, @CONST_MAX_YEARS, GETDATE()) OR @endData < @pDataOraInizio)
		BEGIN
			RAISERROR(N'Il paramentro @pRecurrency contiene una data fine non valida', 16, 1);
			RETURN -8;
		END
		-- Se specificati il numero di ripetizioni, non possono 
		IF @numRipetizioni IS NOT NULL AND @numRipetizioni > @CONST_MAX_RIPETIZ
		BEGIN
			RAISERROR(N'Il paramentro @pRecurrency contiene un numero di ripetizioni troppo alto. (Max: 500)', 16, 1);
			RETURN -8;		
		END
		--TODO: GESTIRE IL NUMERO DI RIPETIZIONI OLTRE CHE LA ENDDATE
		-- Generazioni eventi figli
		IF @recurrency = 'daily'
		BEGIN
			INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, 
									DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent, CancellazioneConsentita, WaitListDisponibile)
				SELECT @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, 							
						DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), g.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME)),
						@pIstruttore, @pPosti, @pPosti,
						-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
						DATEADD(SECOND, 
									@diffCancellabile,
									DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), g.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
						-- Stessa cosa per DataAperturaIscrizioni
						DATEADD(SECOND, 
									@diffAperturaIscr,
									DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), g.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
						-- Stessa cosa per DataChiusuraIscrizioni
						DATEADD(SECOND, 
									@diffChiusuraIscr,
									DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), g.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
						@pNote, 
						@pUserIdOwner, 
						@pIdParent,
						@pCancellazionePossib,
						@pWaitListDisponibile
				FROM [utils].[Numbers] n
					INNER JOIN [utils].[Giorni] g ON g.[Data] = DATEADD(DAY, n.Number, @pDataOraInizio)
				WHERE ((@endData IS NULL) OR (g.Data <= @endData))
				AND ((@numRipetizioni IS NULL) OR (@numRipetizioni < n.Number))
		END
		ELSE IF @recurrency = 'monthly'
		BEGIN
			INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, 
								   DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent, CancellazioneConsentita, WaitListDisponibile)
				SELECT @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation,
						-- DataInizio = DataInizioOriginale + i-esimo mese
						DATEADD(MONTH, n.Number, @pDataOraInizio),
						@pIstruttore, @pPosti, @pPosti,
						-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
						DATEADD(SECOND, 
									@diffCancellabile,
									DATEADD(MONTH, n.Number, @pDataOraInizio)),
						-- Stessa cosa per DataAperturaIscrizioni
						DATEADD(SECOND, 
									@diffAperturaIscr,
									DATEADD(MONTH, n.Number, @pDataOraInizio)),
						-- Stessa cosa per DataChiusuraIscrizioni
						DATEADD(SECOND, 
									@diffChiusuraIscr,
									DATEADD(MONTH, n.Number, @pDataOraInizio)),
						@pNote, 
						@pUserIdOwner, 
						@pIdParent,
						@pCancellazionePossib,
						@pWaitListDisponibile
			FROM [utils].[Numbers] AS n
			WHERE n.Number > 0
			AND ((@endData IS NULL) OR (n.Number < DATEDIFF(MONTH, @pDataOraInizio, @endData)))
			AND ((@numRipetizioni IS NULL) OR (n.Number <@numRipetizioni))
		END
		ELSE IF @recurrency = 'weekly'
		BEGIN				
			--Caso in cui non ci sono DaysOfWeek specificati ==> vuol dire tutte le settimane, lo stesso giorno dell'evento parent (Es: tutti i martedì)
			IF @firstDay IS NULL
			BEGIN
				INSERT INTO @tblWeekDays (WeekDayNum)
					SELECT DATEPART(dw, @pDataOraInizio)
			END
			ELSE
			-- Se invece sono specificati dei DaysOfWeek  => vuol dire tutte le settimane, più giorni la settimana (Es: lun, mer, ven)
			BEGIN
				INSERT INTO @tblWeekDays (WeekDayNum)
				select gi.DayOfWeekNum
					FROM (
						SELECT lower([DayOfWeek]) AS [DayOfWeek]
						FROM OPENJSON( @pRecurrency, '$.DaysOfWeek' ) 
						WITH ([DayOfWeek] NVARCHAR(25) '$')) rg
					-- Prendiamo arbitrariamente la seconda settimana del 2018 per recuperare il DayOfWeekNum
					INNER JOIN [utils].[Giorni] gi ON gi.WeekNum = 2 AND lower(gi.DayOfWeekItaShort) = rg.[DayOfWeek] AND gi.Year = 2018
			END
			IF @endData IS NOT NULL
			BEGIN
				INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, 
										DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent, CancellazioneConsentita, WaitListDisponibile)
					SELECT @pIdCliente, @pTitle, @pIdTipoLezione, @pIdLocation, 							
							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME)),
							@pIstruttore, @pPosti, @pPosti,
							-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
							DATEADD(SECOND, 
										@diffCancellabile,
										DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
							-- Stessa cosa per DataAperturaIscrizioni
							DATEADD(SECOND, 
										@diffAperturaIscr,
										DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
							-- Stessa cosa per DataChiusuraIscrizioni
							DATEADD(SECOND, 
										@diffChiusuraIscr,
										DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))),
							@pNote, 
							@pUserIdOwner, 
							@pIdParent,
							@pCancellazionePossib,
							@pWaitListDisponibile
					FROM [utils].[Giorni] AS gi
						INNER JOIN @tblWeekDays tdw ON tdw.WeekDayNum = gi.DayOfWeekNum
					WHERE gi.Data > @pDataOraInizio
					AND gi.Data <= @endData
			END
			ELSE IF @numRipetizioni IS NOT NULL
			BEGIN
				INSERT INTO Schedules (IdCliente, Title, IdTipoLezione, IdLocation, DataOraInizio, Istruttore, PostiDisponibili, PostiResidui, CancellabileFinoAl, 
										DataAperturaIscrizioni, DataChiusuraIscrizioni, Note, UserIdOwner, IdParent, CancellazioneConsentita, WaitListDisponibile)
				SELECT T.IdCliente, T.Title, T.IdTipoLezione, T.IdLocation, T.DataOraInizio, T.Istruttore, T.PostiIniziali, T.PostiResidui, T.CancellabileFinoAl, 
						T.DataAperturaIscrizioni, T.DataChiusuraIscrizioni, T.Note, T.UserIdOwner, T.IdParent, T.CancellazionePossib, T.WaitListDisponibile
				FROM (
					SELECT @pIdCliente AS IdCliente, @pTitle AS Title, @pIdTipoLezione AS IdTipoLezione, @pIdLocation AS IdLocation,
							DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME)) AS DataOraInizio,
							@pIstruttore AS Istruttore, @pPosti AS PostiIniziali, @pPosti AS PostiResidui,
							-- Usiamo la stessa differenza in secondi tra CancellabileFinoAl e DataOraInizio per le date successive
							DATEADD(SECOND, 
										@diffCancellabile,
										DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))) AS CancellabileFinoAl,
							-- Stessa cosa per DataAperturaIscrizioni
							DATEADD(SECOND, 
										@diffAperturaIscr,
										DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))) AS DataAperturaIscrizioni,
							-- Stessa cosa per DataChiusuraIscrizioni
							DATEADD(SECOND, 
										@diffChiusuraIscr,
										DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))) AS DataChiusuraIscrizioni,
							@pNote AS Note, 
							@pUserIdOwner AS UserIdOwner, 
							@pIdParent AS IdParent,
							@pCancellazionePossib AS CancellazionePossib,
							@pWaitListDisponibile AS WaitListDisponibile,
							ROW_NUMBER() OVER (ORDER BY DATEADD(DAY, DATEDIFF(DAY, CONVERT(DATE, '19000101', 112), gi.[Data]), CAST(CAST(@pDataOraInizio AS TIME) AS DATETIME))) AS RowNumeber
					FROM [utils].[Giorni] AS gi
						INNER JOIN @tblWeekDays tdw ON tdw.WeekDayNum = gi.DayOfWeekNum
					WHERE gi.Data > @pDataOraInizio
					AND gi.Data <= @endData
					) T
					WHERE T.RowNumeber <= @numRipetizioni
			END
		END
END