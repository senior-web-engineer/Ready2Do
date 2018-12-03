CREATE PROCEDURE [dbo].[Schedules_Lista]
	@pIdCliente			INT,
	@pStartDate			DATETIME = NULL,
	@pEndDate			DATETIME = NULL,
	@pIdLocation		INT = NULL,
	@pTipoLezione		INT = NULL,
	@pSoloPostiDispon	BIT = NULL,
	@pSoloIscrizAperte	BIT = NULL,
	@pPageSize			INT = 25,
	@pPageNumber		INT = 1,
	@pSortColumn		VARCHAR(50) = NULL,
	@pOrderAscending	BIT = 1,
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	--TODO: Costruire la select dinamica con i filtri opportuni
	--SELECT	s.Id,
	--		s.IdCliente,
	--		s.Title,
	--		s.IdTipoLezione,
	--		s.IdLocation,
	--		s.DataOraInizio,
	--		s.Istruttore,
	--		s.PostiDisponibili,
	--		s.PostiResidui,
	--		s.CancellabileFinoAl,
	--		s.DataAperturaIscrizioni,
	--		s.DataChiusuraIscrizioni,
	--		s.DataCancellazione,
	--		s.UserIdOwner,
	--		s.Note,
	--		COALESCE(s.Recurrency, sp.Recurrency) AS Recurrency,
	--		s.IdParent,
	--		l.Nome AS NomeLocation,
	--		l.CapienzaMax AS CapienzaMaxLocation,
	--		l.Descrizione AS DescrizioneLocation,
	--		tl.Nome AS NomeTipoLezione,
	--		tl.Descrizione AS DescrizioneTipoLezione,
	--		tl.Durata AS DurataTipoLezione,
	--		tl.LimiteCancellazioneMinuti AS LimiteCancellazioneMinutiTipoLezione,
	--		tl.Livello AS LivelloTipoLezione,
	--		tl.MaxPartecipanti AS MaxPartecipantiTipoLezione,
	--		tl.DataCreazione AS TipoLezioneDataCreazione,
	--		tl.DataCancellazione AS TipoLezioneDataCancellazione
	--FROM [Schedules] s
	--	LEFT JOIN [Schedules] sp ON s.IdParent = sp.Id
	--INNER JOIN [Locations] l ON s.IdLocation = l.Id
	--INNER JOIN [TipologieLezioni] tl ON s.IdTipoLezione = tl.Id
	--WHERE s.Id = @pId
	--AND s.IdCliente = @pIdCliente
	--AND ((COALESCE(@pIncludeDeleted, 0) =1) OR ( s.DataCancellazione IS NULL))
END