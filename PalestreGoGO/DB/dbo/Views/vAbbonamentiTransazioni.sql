CREATE VIEW [dbo].[vAbbonamentiTransazioni]
	AS 
	SELECT 	[Id]					AS IdAbbonamentiTransazioni ,
			[IdAbbonamento]			AS IdAbbonamentoAbbonamentiTransazioni,
			[DataTransazione]		AS DataTransazioneAbbonamentiTransazioni,
			[Testo]					AS TestoAbbonamentiTransazioni
	FROM AbbonamentiTransazioni