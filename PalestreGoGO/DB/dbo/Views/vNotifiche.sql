CREATE VIEW [dbo].[vNotifiche]
AS 
SELECT   n.Id						  AS IdNotifiche
		,n.IdTipo					  AS IdTipoNotifiche
		,n.UserId					  AS UserIdNotifiche
		,n.IdCliente				  AS IdClienteNotifiche
		,n.Titolo					  AS TitoloNotifiche
		,n.Testo					  AS TestoNotifiche
		,n.DataCreazione			  AS DataCreazioneNotifiche
		,n.DataInizioVisibilita		  AS DataInizioVisibilitaNotifiche
		,n.DataFineVisibilita		  AS DataFineVisibilitaNotifiche
		,n.DataDismissione			  AS DataDismissioneNotifiche
		,n.DataPrimaVisualizzazione	  AS DataPrimaVisualizzazioneNotifiche
		,n.ActionUrl				  AS ActionUrlNotifiche
		,tn.*
FROM Notifiche n
	INNER JOIN vTipologieNotifiche tn ON n.IdTipo = tn.IdTipologieNotifiche