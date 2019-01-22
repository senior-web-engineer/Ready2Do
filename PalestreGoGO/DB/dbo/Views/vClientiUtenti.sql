CREATE VIEW [dbo].[vClientiUtenti]
AS 
	SELECT  cu.UserId				AS UserIdClientiUtenti,
			cu.IdCliente			AS IdClienteClientiUtenti,
			cu.Cognome				AS CognomeClientiUtenti,
			cu.Nome					AS NomeClientiUtenti,
			cu.UserDisplayName		AS UserDisplayNameClientiUtenti,
			cu.DataAggiornamento	AS DataAggiornamentoClientiUtenti,
			cu.DataCancellazione	AS DataCancellazioneClientiUtenti,
			cu.DataCreazione		AS DataCreazioneClientiUtenti
	FROM ClientiUtenti cu
