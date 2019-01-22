CREATE VIEW [dbo].[vSchedules]
AS 
SELECT 
	 s.Id							AS IdSchedules
	,s.IdCliente					AS IdClienteSchedules
	,s.Title						AS TitleSchedules
	,s.IdTipoLezione				AS IdTipoLezioneSchedules
	,s.IdLocation					AS IdLocationSchedules
	,s.DataOraInizio				AS DataOraInizioSchedules
	,s.Istruttore					AS IstruttoreSchedules
	,s.PostiDisponibili				AS PostiDisponibiliSchedules
	,s.PostiResidui					AS PostiResiduiSchedules
	,(s.PostiDisponibili - s.PostiResidui) AS NumPrenotazioniSchedules
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
FROM Schedules s
	LEFT JOIN Schedules sp ON sp.Id = s.IdParent