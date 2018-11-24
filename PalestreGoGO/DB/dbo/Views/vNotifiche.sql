CREATE VIEW [dbo].[vNotifiche]
AS 
SELECT   Id						  AS IdNotifiche
		,IdTipo					  AS IdTipoNotifiche
		,UserId					  AS UserIdNotifiche
		,IdCliente				  AS IdClienteNotifiche
		,Titolo					  AS TitoloNotifiche
		,Testo					  AS TestoNotifiche
		,DataCreazione			  AS DataCreazioneNotifiche
		,DataInizioVisibilita	  AS DataInizioVisibilitaNotifiche
		,DataFineVisibilita		  AS DataFineVisibilitaNotifiche
		,DataDismissione		  AS DataDismissioneNotifiche
		,DataPrimaVisualizzazione AS DataPrimaVisualizzazioneNotifiche
FROM Notifiche