CREATE PROCEDURE [dbo].[Clienti_Utenti_Lista]
	@pIdCliente			INT,
	@pIncludeStato		BIT = 0,
	@pPageSize			INT = 25,
	@pPageNumber		INT = 1,
	@pSortColumn		VARCHAR(50) = NULL,
	@pOrderAscending	BIT = 1

AS
BEGIN
	DECLARE @sql NVARCHAR(MAX);
	DECLARE @sortDirection VARCHAR(10);

	IF @pIdCliente IS NULL
	BEGIN
		RAISERROR('Invalid @pidCliente', 16, 0);
		RETURN -2;		
	END
	SET @pSortColumn = LOWER(COALESCE(@pSortColumn, 'Cognome'));
	-- Check colonna di ordinamento (avoid injection)
	IF @pSortColumn NOT IN (N'cognome', N'nome', N'datacreazione')
	BEGIN
		RAISERROR('Invalid parameter for @pSortColumn: %s', 11, 1, @pSortColumn);
		RETURN -1;
	END 

	SET @pSortColumn = CASE @pSortColumn
							WHEN 'cognome' THEN QUOTENAME('CognomeClientiUtenti')
							WHEN 'nome' THEN QUOTENAME('NomeClientiUtenti')
							WHEN 'datacreazione' THEN QUOTENAME('DataCreazioneClientiUtenti')
						END;
	SET @sortDirection = CASE COALESCE(@pOrderAscending, 1) WHEN 1 THEN 'ASC' ELSE 'DESC' END
	SET @pPageSize = COALESCE(@pPageSize, 25);
	SET @pPageNumber = COALESCE(@pPageNumber, 1);

	CREATE TABLE  #tblUsers(
		[IdClienteClientiUtenti]			INT					NOT NULL,
		[UserIdClientiUtenti]				VARCHAR(50)			NOT NULL,
		[NomeClientiUtenti]					NVARCHAR(100)		NOT NULL,
		[CognomeClientiUtenti]				NVARCHAR(100)		NOT NULL,
		[UserDisplayNameClientiUtenti]		NVARCHAR(100)		NOT NULL,
		[DataAggiornamentoClientiUtenti]	DATETIME2(2)		NOT NULL,
		[DataCreazioneClientiUtenti]		DATETIME2(2)		NOT NULL,
		[DataCancellazioneClientiUtenti]	DATETIME2(2)		NULL );

	SET @sql = N'	INSERT INTO #tblUsers (IdClienteClientiUtenti, UserIdClientiUtenti, NomeClientiUtenti, CognomeClientiUtenti, UserDisplayNameClientiUtenti, 
											DataAggiornamentoClientiUtenti, DataCreazioneClientiUtenti, DataCancellazioneClientiUtenti)
						SELECT  cu.IdClienteClientiUtenti 
								,cu.UserIdClientiUtenti
								,cu.NomeClientiUtenti 
								,cu.CognomeClientiUtenti 
								,cu.UserDisplayNameClientiUtenti
								,cu.DataAggiornamentoClientiUtenti
								,cu.DataCreazioneClientiUtenti 
								,cu.DataCancellazioneClientiUtenti
						FROM [dbo].[vClientiUtenti] cu
						WHERE cu.IdClienteClientiUtenti = @pIdCliente
						AND DataCancellazioneClientiUtenti IS NULL --Escludiamo i cancellati
						ORDER BY ' + @pSortColumn  + ' ' + @sortDirection + '
								OFFSET @pPageSize * (@pPageNumber - 1) ROWS
								FETCH NEXT @pPageSize ROWS ONLY';

	EXEC sp_executesql @sql, N'@pPageSize int, @pPageNumber int, @pIdCliente int', @pPageSize=@pPageSize, @pPageNumber=@pPageNumber, @pIdCliente=@pIdCliente

	IF COALESCE(@pIncludeStato, 0) = 1
	BEGIN
		;WITH cte_abbonamenti_att AS(
			SELECT u.IdClienteClientiUtenti, 
					u.UserIdClientiUtenti,
					CASE
						WHEN auAtt.IdAbbonamentiUtenti IS NOT NULL THEN CAST(1 AS tinyint)
						ELSE CAST (0 AS tinyint)
					END AS HasAbbonamentoAttivo,
					CASE 
						WHEN auAtt.IdAbbonamentiUtenti IS NOT NULL AND auAtt.ImportoPagatoAbbonamentiUtenti = auAtt.ImportoAbbonamentiUtenti THEN CAST(3 AS TINYINT) --Pagato
						WHEN auAtt.IdAbbonamentiUtenti IS NOT NULL AND auAtt.ImportoPagatoAbbonamentiUtenti = 0 AND auAtt.ImportoAbbonamentiUtenti > 0 THEN CAST(1 AS TINYINT) --Da Pagare
						WHEN auAtt.IdAbbonamentiUtenti IS NULL THEN CAST(0 AS TINYINT) --nulla da pagare
						ELSE CAST(2 AS TINYINT) -- Parzialmente pagato
					END As StatoPagamentoAbbonamentoAttivo
			FROM #tblUsers u 
				LEFT JOIN [dbo].[vAbbonamentiUtenti] auAtt ON auAtt.IdClienteAbbonamentiUtenti = u.IdClienteClientiUtenti AND auAtt.UserIDAbbonamentiUtenti = u.UserIdClientiUtenti 
													AND auAtt.DataInizioValiditaAbbonamentiUtenti <= SYSDATETIME() AND  auAtt.DataCancellazioneAbbonamentiUtenti IS NULL 
													AND auAtt.ScadenzaAbbonamentiUtenti > SYSDATETIME() -- consideriamo attivi solo i NON Cancellati e NON Scaduti e validi ad oggi
		),
		cte_abbonamenti_old AS(
				SELECT u.IdClienteClientiUtenti, 
					   u.UserIdClientiUtenti,
					   CASE
							WHEN auOld.IdAbbonamentiUtenti IS NOT NULL AND auOld.ScadenzaAbbonamentiUtenti < SYSDATETIME() THEN CAST(1 AS tinyint)
							ELSE CAST (0 AS tinyint)
					   END AS HasAbbonamentoScaduto,
					   CASE
							WHEN auOld.IdAbbonamentiUtenti IS NOT NULL AND auOld.DataCancellazioneAbbonamentiUtenti IS NOT NULL THEN CAST(1 AS tinyint)
							ELSE CAST (0 AS tinyint)
					   END AS HasAbbonamentoCancellato
				FROM #tblUsers u 
					LEFT JOIN [dbo].[vAbbonamentiUtenti] auOld ON auOld.IdClienteAbbonamentiUtenti = u.IdClienteClientiUtenti AND auOld.UserIDAbbonamentiUtenti = u.UserIdClientiUtenti 
														AND auOld.DataInizioValiditaAbbonamentiUtenti <= SYSDATETIME() AND 
														((auOld.DataCancellazioneAbbonamentiUtenti IS NOT NULL) OR  (auOld.ScadenzaAbbonamentiUtenti < SYSDATETIME()))
		),
		cte_certificato as(
			SELECT u.IdClienteClientiUtenti,
				   u.UserIdClientiUtenti,
				   CASE 
						WHEN c.IdClientiUtentiCertificati IS NOT NULL AND c.DataScadenzaClientiUtentiCertificati > SYSDATETIME() 
												AND c.DataCancellazioneClientiUtentiCertificati IS NULL THEN CAST(1 AS tinyint)
						ELSE CAST(0 AS tinyint)
				   END AS HasCertificatoValido,
				   CASE 
						WHEN c.IdClientiUtentiCertificati IS NOT NULL AND c.DataScadenzaClientiUtentiCertificati < SYSDATETIME() 
												AND c.DataCancellazioneClientiUtentiCertificati IS NULL THEN CAST(1 AS tinyint)
						ELSE CAST(0 AS tinyint)
				   END AS HasCertificatoScaduto
			FROM #tblUsers u 
				LEFT JOIN [dbo].[vClientiUtentiCertificati] c ON u.IdClienteClientiUtenti = c.IdClientiUtentiCertificati AND u.UserIdClientiUtenti = c.UserIdClientiUtentiCertificati
		)
		SELECT u.IdClienteClientiUtenti,
			   u.UserIdClientiUtenti,
			   u.CognomeClientiUtenti,
			   u.NomeClientiUtenti,
			   u.UserDisplayNameClientiUtenti,
			   u.DataCreazioneClientiUtenti,
			   u.DataAggiornamentoClientiUtenti,
			   u.DataCancellazioneClientiUtenti, -- sarà sempre NULL avendo escluso i cancellati nella query a monte

			   abbAtt.HasAbbonamentoAttivo,
			   abbAtt.StatoPagamentoAbbonamentoAttivo,
			   abbOld.HasAbbonamentoCancellato,
			   abbOld.HasAbbonamentoScaduto,
			   cer.HasCertificatoScaduto,
			   cer.HasCertificatoValido
		FROM #tblUsers u
			inner join (SELECT IdClienteClientiUtenti, UserIdClientiUtenti, 
							MAX(HasAbbonamentoAttivo) AS HasAbbonamentoAttivo, 
							MIN(StatoPagamentoAbbonamentoAttivo) AS StatoPagamentoAbbonamentoAttivo
						FROM cte_abbonamenti_att 
						GROUP BY IdClienteClientiUtenti, UserIdClientiUtenti
						) abbAtt ON u.IdClienteClientiUtenti = abbAtt.IdClienteClientiUtenti AND u.UserIdClientiUtenti = abbAtt.UserIdClientiUtenti
			inner join (SELECT IdClienteClientiUtenti, UserIdClientiUtenti,
								MAX(HasAbbonamentoScaduto) AS HasAbbonamentoScaduto,
								MAX(HasAbbonamentoCancellato) AS HasAbbonamentoCancellato
						FROM cte_abbonamenti_old
						GROUP BY IdClienteClientiUtenti, UserIdClientiUtenti
						)abbOld ON u.IdClienteClientiUtenti = abbOld.IdClienteClientiUtenti AND u.UserIdClientiUtenti = abbOld.UserIdClientiUtenti
			inner join (SELECT IdClienteClientiUtenti, UserIdClientiUtenti,
								MAX(HasCertificatoValido) AS HasCertificatoValido,
								MAX(HasCertificatoScaduto) AS HasCertificatoScaduto
						FROM cte_certificato
						GROUP BY IdClienteClientiUtenti, UserIdClientiUtenti
						) cer ON u.IdClienteClientiUtenti = cer.IdClienteClientiUtenti AND u.UserIdClientiUtenti = cer.UserIdClientiUtenti
	--Non riapplichiamo l'ordinamento perché la #tblUsers dovrebbe già essere ordinata con il criterio voluto
	END
	ELSE
	BEGIN
		SELECT	   u.IdClienteClientiUtenti,
				   u.UserIdClientiUtenti,
				   u.CognomeClientiUtenti,
				   u.NomeClientiUtenti,
				   u.UserDisplayNameClientiUtenti,
				   u.DataCreazioneClientiUtenti,
				   u.DataAggiornamentoClientiUtenti,
				   u.DataCancellazioneClientiUtenti -- sarà sempre NULL avendo escluso i cancellati nella query a monte
			FROM #tblUsers u
	END
END