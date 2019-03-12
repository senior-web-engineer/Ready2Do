CREATE PROCEDURE [dbo].[Clienti_Utenti_Get]
	@pIdCliente			INT,
	@pUserId			VARCHAR(100),
	@pIncludeStato		BIT = 0 --dovrebbe essere obsoleto questo parametro
AS
BEGIN
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
						ELSE CAST(2 AS TINYINT) -- Parzialmente pagato
					END As StatoPagamentoAbbonamentoAttivo
			FROM vClientiUtenti u 
				LEFT JOIN vAbbonamentiUtenti auAtt ON auAtt.IdClienteAbbonamentiUtenti = u.IdClienteClientiUtenti AND auAtt.UserIdAbbonamentiUtenti = u.UserIdClientiUtenti 
													AND auAtt.DataInizioValiditaAbbonamentiUtenti <= SYSDATETIME() AND  auAtt.DataCancellazioneAbbonamentiUtenti IS NULL 
													AND auAtt.ScadenzaAbbonamentiUtenti > SYSDATETIME() -- consideriamo attivi solo i NON Cancellati e NON Scaduti e validi ad oggi
			WHERE u.IdClienteClientiUtenti = @pIdCliente
			AND u.UserIdClientiUtenti = @pUserId
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
				FROM vClientiUtenti u 
					LEFT JOIN vAbbonamentiUtenti auOld ON auOld.IdClienteAbbonamentiUtenti = u.IdClienteClientiUtenti AND auOld.UserIdAbbonamentiUtenti = u.UserIdClientiUtenti 
														AND auOld.DataInizioValiditaAbbonamentiUtenti <= SYSDATETIME() AND 
														((auOld.DataCancellazioneAbbonamentiUtenti IS NOT NULL) OR  (auOld.ScadenzaAbbonamentiUtenti < SYSDATETIME()))
				WHERE u.IdClienteClientiUtenti = @pIdCliente
				AND u.UserIdClientiUtenti = @pUserId

		),
		cte_certificato as(
			SELECT u.IdClienteClientiUtenti,
				   u.UserIdClientiUtenti,
				   CASE 
						WHEN c.IdClientiUtentiCertificati IS NOT NULL AND c.DataScadenzaClientiUtentiCertificati > SYSDATETIME() AND c.DataCancellazioneClientiUtentiCertificati IS NULL THEN CAST(1 AS tinyint)
						ELSE CAST(0 AS tinyint)
				   END AS HasCertificatoValido,
				   CASE 
						WHEN c.IdClientiUtentiCertificati IS NOT NULL AND c.DataScadenzaClientiUtentiCertificati < SYSDATETIME() AND c.DataCancellazioneClientiUtentiCertificati IS NULL THEN CAST(1 AS tinyint)
						ELSE CAST(0 AS tinyint)
				   END AS HasCertificatoScaduto
			FROM vClientiUtenti u 
				LEFT JOIN vClientiUtentiCertificati c ON u.IdClienteClientiUtenti = c.IdClienteClientiUtentiCertificati AND u.UserIdClientiUtenti = c.UserIdClientiUtentiCertificati
			WHERE u.IdClienteClientiUtenti = @pIdCliente
			AND u.UserIdClientiUtenti = @pUserId
		)
		SELECT TOP 1 -- solo l'ultimo record ci interessa
			   u.IdClienteClientiUtenti,
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
		FROM vClientiUtenti u
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
		WHERE u.IdClienteClientiUtenti = @pIdCliente
		AND u.UserIdClientiUtenti = @pUserId
		ORDER BY u.DataCreazioneClientiUtenti DESC -- Prendiamo solo l'ultimo record, quelli storici non ci interessano
	END
	ELSE
	BEGIN
		SELECT TOP 1 -- solo l'ultimo record ci interessa
				   u.IdClienteClientiUtenti,
				   u.UserIdClientiUtenti,
				   u.CognomeClientiUtenti,
				   u.NomeClientiUtenti,
				   u.UserDisplayNameClientiUtenti,
				   u.DataCreazioneClientiUtenti,
				   u.DataAggiornamentoClientiUtenti,
				   u.DataCancellazioneClientiUtenti -- sarà sempre NULL avendo escluso i cancellati nella query a monte
			FROM vClientiUtenti u
		WHERE u.IdClienteClientiUtenti = @pIdCliente
		AND u.UserIdClientiUtenti = @pUserId
		ORDER BY u.DataCreazioneClientiUtenti DESC -- Prendiamo solo l'ultimo record, quelli storici non ci interessano
	END
END