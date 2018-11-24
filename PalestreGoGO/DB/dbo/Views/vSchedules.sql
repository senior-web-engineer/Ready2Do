CREATE VIEW [dbo].[vSchedules]
AS 
SELECT 
	 Id						AS IdSchedules
	,IdCliente				AS IdClienteSchedules
	,Title					AS TitleSchedules
	,IdTipoLezione			AS IdTipoLezioneSchedules
	,IdLocation				AS IdLocationSchedules
	,DataOraInizio			AS DataOraInizioSchedules
	,Istruttore				AS IstruttoreSchedules
	,PostiDisponibili		AS PostiDisponibiliSchedules
	,PostiResidui			AS PostiResiduiSchedules
	,CancellabileFinoAl		AS CancellabileFinoAlSchedules
	,DataChiusuraIscrizioni	AS DataChiusuraIscrizioniSchedules
	,DataCancellazione		AS DataCancellazioneSchedules
	,UserIdOwner			AS UserIdOwnerSchedules
	,Note					AS NoteSchedules
FROM Schedules