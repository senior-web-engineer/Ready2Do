CREATE VIEW [dbo].[vSchedulesFull]
AS 
SELECT 
	 s.Id							AS IdSchedules
	,s.IdCliente					AS IdClienteSchedules
	,s.Title						AS TitleSchedules
	,s.IdTipoLezione				AS IdTipoLezione
	,s.IdLocation					AS IdLocation
	,s.DataOraInizio				AS DataOraInizioSchedules
	,s.Istruttore					AS IstruttoreSchedules
	,s.PostiDisponibili				AS PostiDisponibiliSchedules
	,s.PostiResidui					AS PostiResiduiSchedules
	,s.CancellazioneConsentita		AS CancellazioneConsentitaSchedules
	,s.CancellabileFinoAl			AS CancellabileFinoAlSchedules
	,s.DataAperturaIscrizioni		AS DataAperturaIscrizioniSchedules
	,s.DataChiusuraIscrizioni		AS DataChiusuraIscrizioniSchedules
	,s.DataCancellazione			AS DataCancellazioneSchedules
	,s.UserIdOwner					AS UserIdOwnerSchedules
	,s.Note							AS NoteSchedules
	,s.WaitListDisponibile			AS WaitListDisponibileSchedules
	,s.VisibileDal					AS VisibileDalSchedules
	,s.VisibileFinoAl				AS VisibileFinoAlSchedules
	,s.IdParent						AS IdParentSchedules
	,COALESCE(s.Recurrency, sp.Recurrency) AS RecurrencySchedules
	,s.DataCreazione				AS DataCreazioneSchedules

	,l.Nome							AS NomeLocation
	,l.Descrizione					AS DescrizioneLocation
	,l.CapienzaMax					AS CapienzaMaxLocation
	,l.Colore						AS ColoreLocation
	,l.DataCancellazione			AS DataCancellazioneLocation
	,l.DataCreazione				AS DataCreazioneLocation
	,l.IconUrl						AS IconUrlLocation
	,l.ImageUrl						AS ImageUrlLocation

	,tl.Nome						AS NomeTipoLezione
	,tl.DataCreazione				AS DataCreazioneTipoLezione
	,tl.Descrizione					AS DescrizioneTipoLezione
	,tl.DataCancellazione			AS DataCancellazioneTipoLezione
	,tl.Durata						AS DurataTipoLezione
	,tl.LimiteCancellazioneMinuti	AS LimiteCancallazioneTipoLezione
	,tl.Livello						AS LivelloTipoLezione
	,tl.MaxPartecipanti				AS MaxPartecipantiTipoLezione
	,tl.Prezzo						AS PrezzoTipoLezione

FROM Schedules s
	INNER JOIN TipologieLezioni tl ON tl.Id = s.Id AND tl.IdCliente = s.IdCliente
	INNER JOIN Locations l ON l.Id = s.IdLocation AND l.IdCliente = s.IdCliente
	LEFT JOIN Schedules sp ON sp.Id = s.IdParent