CREATE VIEW [dbo].[vAppuntamenti]
	AS 
	SELECT a.Id					AS IdAppuntamenti,
		   a.IdCliente			AS IdClienteAppuntamenti,
		   a.UserId				as UserIdAppuntamenti,
		   a.ScheduleId			as ScheduleIdAppuntamenti,
		   a.IdAbbonamento		AS IdAbbonamentoAppuntamenti,
		   a.DataPrenotazione	AS DataPrenotazioneAppuntamenti,
		   a.DataCancellazione	as DataCancellazioneAppuntamenti,
		   a.Note				as NoteAppuntamenti,
		   a.Nominativo			as NominativoAppuntamenti
	FROM Appuntamenti a
