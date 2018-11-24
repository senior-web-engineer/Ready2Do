CREATE VIEW [dbo].[vClientiUtentiCertificati]
AS 
	SELECT   Id					AS IdClientiUtentiCertificati
			,IdCliente			AS IdClienteClientiUtentiCertificati
			,UserId				AS UserIdClientiUtentiCertificati
			,DataPresentazione	AS DataPresentazioneClientiUtentiCertificati
			,DataScadenza		AS DataScadenzaClientiUtentiCertificati
			,Note				AS NoteClientiUtentiCertificati
			,DataCancellazione	AS DataCancellazioneClientiUtentiCertificati
	FROM ClientiUtentiCertificati
