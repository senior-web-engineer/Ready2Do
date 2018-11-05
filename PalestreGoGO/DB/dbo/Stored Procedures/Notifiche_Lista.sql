/*
Ritorna la lista di notifiche per l'utente. Se specificato un IdCliente ritorna solo le notifiche associate a quel cliente.
Il parametro @pStatoNotifica indica quali notifiche si volgiono in output e può assumere uno di questi valori:
- 0 = Tutte
- 1 = SoloAttive (non dismesse) ==> DEFAULT
- 2 = SoloNonLette (DataDismissione NULL + DataPrimaVisualizzazione NULL)
*/
CREATE PROCEDURE [dbo].[Notifiche_Lista]
	@pIdUtente		VARCHAR(100) = NULL,
	@pIdCliente		INT,
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
			n.IdUtente,
			tn.Id AS IdTipo,
			tn.Code	AS Code,
			tn.UserDismissable,
			tn.AutoDismissAfter,
			tn.Priority
	FROM Notifiche n
		INNER JOIN TipologieNotifiche tn ON n.IdTipo = tn.Id
	WHERE (((COALESCE(@pStatoNotifica, 1) =1) AND (n.DataDismissione IS NULL)) OR
		   ((@pStatoNotifica = 2) AND  (n.DataDismissione IS NULL) AND (n.DataPrimaVisualizzazione IS NULL)) OR
		   (@pStatoNotifica = 0))
END