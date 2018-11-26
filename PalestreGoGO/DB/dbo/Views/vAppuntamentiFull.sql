CREATE VIEW [dbo].[vAppuntamentiFull]
	AS 
 SELECT		   ap.Id,
			   ap.UserId,
			   ap.IdCliente,
			   ap.DataPrenotazione,
			   ap.DataCancellazione,
			   ap.IdAbbonamento,
			   ap.Nominativo,
			   ap.Note,
			   ap.ScheduleId,
			   sc.CancellabileFinoAl,
			   sc.DataCancellazione as DataCancellazioneSchedule,
			   sc.DataChiusuraIscrizioni,
			   sc.DataOraInizio,
			   sc.IdTipoLezione,
			   sc.Istruttore,
			   sc.Note AS NoteSchedule,
			   sc.PostiDisponibili,
			   sc.PostiResidui,
			   sc.Title,
			   sc.UserIdOwner,
			   tl.Id AS IdTipologiaLezione,
			   tl.Nome as NomeTipologiaLezione,
			   tl.Descrizione AS DescrizioneTipologiaLezione,
			   tl.Durata AS Durata,
			   tl.MaxPartecipanti,
			   tl.LimiteCancellazioneMinuti,
			   tl.Livello,
			   tl.DataCancellazione AS DataCancellazioneTipologiaLezione,
			   tl.DataCreazione AS DataCreazioneTipologiaLezione,
			   lo.Id AS IdLocation,
			   lo.Nome AS NomeLocation,
			   lo.Descrizione AS DescrizioneLocation,
			   lo.CapienzaMax
		FROM Appuntamenti ap
			INNER JOIN Schedules sc ON AP.ScheduleId = sc.Id
			INNER JOIN TipologieLezioni tl ON sc.IdTipoLezione = tl.Id
			INNER JOIN Locations lo ON sc.IdLocation = lo.Id