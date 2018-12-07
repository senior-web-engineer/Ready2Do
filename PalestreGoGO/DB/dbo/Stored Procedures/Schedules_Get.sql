CREATE PROCEDURE [dbo].[Schedules_Get]
	@pId				INT,
	@pIdCliente			INT,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT	s.Id,
			s.IdCliente,
			s.Title,
			s.IdTipoLezione,
			s.IdLocation,
			s.DataOraInizio,
			s.Istruttore,
			s.PostiDisponibili,
			s.PostiResidui,
			s.CancellazioneConsentita,
			s.CancellabileFinoAl,
			s.DataAperturaIscrizioni,
			s.DataChiusuraIscrizioni,
			s.DataCancellazione,
			s.UserIdOwner,
			s.Note,
			s.WaitListDisponibile,
			s.VisibileDal,
			s.VisibileFinoAl,
			COALESCE(s.Recurrency, sp.Recurrency) AS Recurrency,
			s.IdParent,
			l.Nome AS NomeLocation,
			l.CapienzaMax AS CapienzaMaxLocation,
			l.Descrizione AS DescrizioneLocation,
			tl.Nome AS NomeTipoLezione,
			tl.Descrizione AS DescrizioneTipoLezione,
			tl.Durata AS DurataTipoLezione,
			tl.LimiteCancellazioneMinuti AS LimiteCancellazioneMinutiTipoLezione,
			tl.Livello AS LivelloTipoLezione,
			tl.MaxPartecipanti AS MaxPartecipantiTipoLezione,
			tl.DataCreazione AS TipoLezioneDataCreazione,
			tl.DataCancellazione AS TipoLezioneDataCancellazione
	FROM [Schedules] s
		LEFT JOIN [Schedules] sp ON s.IdParent = sp.Id
	INNER JOIN [Locations] l ON s.IdLocation = l.Id
	INNER JOIN [TipologieLezioni] tl ON s.IdTipoLezione = tl.Id
	WHERE s.Id = @pId
	AND s.IdCliente = @pIdCliente
	AND ((COALESCE(@pIncludeDeleted, 0) =1) OR ( s.DataCancellazione IS NULL))
END