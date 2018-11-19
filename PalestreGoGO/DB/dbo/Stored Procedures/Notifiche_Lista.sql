/*
Ritorna la lista di notifiche per l'utente. Se specificato un IdCliente ritorna solo le notifiche associate a quel cliente.
Il parametro @pStatoNotifica indica quali notifiche si volgiono in output e può assumere uno di questi valori:
- 0 = Tutte
- 1 = SoloAttive (non dismesse) ==> DEFAULT
- 2 = SoloNonLette (DataDismissione NULL + DataPrimaVisualizzazione NULL)
*/
CREATE PROCEDURE [dbo].[Notifiche_Lista]
	@pUserId		VARCHAR(100) = NULL,
	@pIdCliente		INT = NULL,
	@pStatoNotifica	TINYINT = 1
AS
BEGIN
	SELECT	n.Id, 
			n.IdCliente,
			n.Titolo,
			n.Testo,
			n.DataCreazione,
			n.DataDismissione,
			n.DataPrimaVisualizzazione,
			n.DataInizioVisibilita,
			n.DataFineVisibilita,
			n.UserId,
			tn.Id AS IdTipo,
			tn.Code	AS Code,
			tn.UserDismissable,
			tn.AutoDismissAfter,
			tn.Priority
	FROM Notifiche n
		INNER JOIN TipologieNotifiche tn ON n.IdTipo = tn.Id
	WHERE (@pIdCliente IS NULL OR @pIdCliente = n.IdCliente) AND
		(((COALESCE(@pStatoNotifica, 1) =1) AND (n.DataDismissione IS NULL) AND
				(COALESCE(n.DataFineVisibilita, SYSDATETIME()) >= SYSDATETIME())) 
		   OR ((@pStatoNotifica = 2) AND  
				(n.DataDismissione IS NULL) AND (n.DataPrimaVisualizzazione IS NULL) AND
				(COALESCE(n.DataFineVisibilita, SYSDATETIME()) >= SYSDATETIME())
		   ) 
		   OR (@pStatoNotifica = 0))
END