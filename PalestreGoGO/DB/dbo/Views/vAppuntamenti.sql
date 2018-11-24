CREATE VIEW [dbo].[vAppuntamenti]
	AS 
	SELECT a.Id					AS IdAppuntamento,
		   a.IdCliente			AS IdClienteAppuntamento,
		   a.UserId				as UserIdAppuntamento,
		   a.ScheduleId			as ScheduleIdAppuntamento,
		   a.IdAbbonamento		AS IdAbbonamentoAppuntamento,
		   a.DataPrenotazione	AS DataPrenotazioneAppuntamento,
		   a.DataCancellazione	as DataCancellazioneAppuntamento,
		   a.Note				as NoteAppuntamento,
		   a.Nominativo			as NominativoAppuntamento		   
	FROM Appuntamenti a
