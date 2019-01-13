CREATE PROCEDURE [dbo].[Clienti_Utenti_Get]
	@pIdCliente			INT,
	@pUserId			VARCHAR(100),
	@pIncludeStato		BIT = 0 --dovrebbe essere obsoleto questo parametro
AS
BEGIN
	IF COALESCE(@pIncludeStato, 0) = 1
	BEGIN
		;WITH cte_abbonamenti_att AS(
			SELECT u.IdCliente, 
					u.UserId,
					CASE
						WHEN auAtt.Id IS NOT NULL THEN CAST(1 AS tinyint)
						ELSE CAST (0 AS tinyint)
					END AS HasAbbonamentoAttivo,
					CASE 
						WHEN auAtt.Id IS NOT NULL AND auAtt.ImportoPagato = auAtt.Importo THEN CAST(3 AS TINYINT) --Pagato
						WHEN auAtt.Id IS NOT NULL AND auAtt.ImportoPagato = 0 AND auAtt.Importo > 0 THEN CAST(1 AS TINYINT) --Da Pagare
						ELSE CAST(2 AS TINYINT) -- Parzialmente pagato
					END As StatoPagamentoAbbonamentoAttivo
			FROM ClientiUtenti u 
				LEFT JOIN AbbonamentiUtenti auAtt ON auAtt.IdCliente = u.IdCliente AND auAtt.UserId = u.UserId 
													AND auAtt.DataInizioValidita <= SYSDATETIME() AND  auAtt.DataCancellazione IS NULL 
													AND auAtt.Scadenza > SYSDATETIME() -- consideriamo attivi solo i NON Cancellati e NON Scaduti e validi ad oggi
			WHERE u.IdCliente = @pIdCliente
			AND u.UserId = @pUserId
		),
		cte_abbonamenti_old AS(
				SELECT u.IdCliente, 
					   u.UserId,
					   CASE
							WHEN auOld.Id IS NOT NULL AND auOld.Scadenza < SYSDATETIME() THEN CAST(1 AS tinyint)
							ELSE CAST (0 AS tinyint)
					   END AS HasAbbonamentoScaduto,
					   CASE
							WHEN auOld.Id IS NOT NULL AND auOld.DataCancellazione IS NOT NULL THEN CAST(1 AS tinyint)
							ELSE CAST (0 AS tinyint)
					   END AS HasAbbonamentoCancellato
				FROM ClientiUtenti u 
					LEFT JOIN AbbonamentiUtenti auOld ON auOld.IdCliente = u.IdCliente AND auOld.UserId = u.UserId 
														AND auOld.DataInizioValidita <= SYSDATETIME() AND 
														((auOld.DataCancellazione IS NOT NULL) OR  (auOld.Scadenza < SYSDATETIME()))
				WHERE u.IdCliente = @pIdCliente
				AND u.UserId = @pUserId

		),
		cte_certificato as(
			SELECT u.IdCliente,
				   u.UserId,
				   CASE 
						WHEN c.Id IS NOT NULL AND c.DataScadenza > SYSDATETIME() AND c.DataCancellazione IS NULL THEN CAST(1 AS tinyint)
						ELSE CAST(0 AS tinyint)
				   END AS HasCertificatoValido,
				   CASE 
						WHEN c.Id IS NOT NULL AND c.DataScadenza < SYSDATETIME() AND c.DataCancellazione IS NULL THEN CAST(1 AS tinyint)
						ELSE CAST(0 AS tinyint)
				   END AS HasCertificatoScaduto
			FROM ClientiUtenti u 
				LEFT JOIN ClientiUtentiCertificati c ON u.IdCliente = c.IdCliente AND u.UserId = c.UserId
			WHERE u.IdCliente = @pIdCliente
			AND u.UserId = @pUserId
		)
		SELECT TOP 1 -- solo l'ultimo record ci interessa
			   u.IdCliente,
			   u.UserId,
			   u.Cognome,
			   u.Nome,
			   u.UserDisplayName,
			   u.DataCreazione,
			   u.DataAggiornamento,
			   u.DataCancellazione, -- sarà sempre NULL avendo escluso i cancellati nella query a monte

			   abbAtt.HasAbbonamentoAttivo,
			   abbAtt.StatoPagamentoAbbonamentoAttivo,
			   abbOld.HasAbbonamentoCancellato,
			   abbOld.HasAbbonamentoScaduto,
			   cer.HasCertificatoScaduto,
			   cer.HasCertificatoValido
		FROM ClientiUtenti u
			inner join (SELECT IdCliente, UserId, 
							MAX(HasAbbonamentoAttivo) AS HasAbbonamentoAttivo, 
							MIN(StatoPagamentoAbbonamentoAttivo) AS StatoPagamentoAbbonamentoAttivo
						FROM cte_abbonamenti_att 
						GROUP BY IdCliente, UserId
						) abbAtt ON u.IdCliente = abbAtt.IdCliente AND u.UserId = abbAtt.UserId
			inner join (SELECT IdCliente, UserId,
								MAX(HasAbbonamentoScaduto) AS HasAbbonamentoScaduto,
								MAX(HasAbbonamentoCancellato) AS HasAbbonamentoCancellato
						FROM cte_abbonamenti_old
						GROUP BY IdCliente, UserId
						)abbOld ON u.IdCliente = abbOld.IdCliente AND u.UserId = abbOld.UserId
			inner join (SELECT IdCliente, UserId,
								MAX(HasCertificatoValido) AS HasCertificatoValido,
								MAX(HasCertificatoScaduto) AS HasCertificatoScaduto
						FROM cte_certificato
						GROUP BY IdCliente, UserId
						) cer ON u.IdCliente = cer.IdCliente AND u.UserId = cer.UserId
		WHERE u.IdCliente = @pIdCliente
		AND u.UserId = @pUserId
		ORDER BY DataCreazione DESC -- Prendiamo solo l'ultimo record, quelli storici non ci interessano
	END
	ELSE
	BEGIN
		SELECT TOP 1 -- solo l'ultimo record ci interessa
				   u.IdCliente,
				   u.UserId,
				   u.Cognome,
				   u.Nome,
				   u.UserDisplayName,
				   u.DataCreazione,
				   u.DataAggiornamento,
				   u.DataCancellazione -- sarà sempre NULL avendo escluso i cancellati nella query a monte
			FROM ClientiUtenti u
		WHERE u.IdCliente = @pIdCliente
		AND u.UserId = @pUserId
		ORDER BY DataCreazione DESC -- Prendiamo solo l'ultimo record, quelli storici non ci interessano
	END
END