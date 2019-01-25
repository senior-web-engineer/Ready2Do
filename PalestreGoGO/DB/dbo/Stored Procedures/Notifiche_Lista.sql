/*
Ritorna la lista di notifiche per l'utente. Se specificato un IdCliente ritorna solo le notifiche associate a quel cliente.
Il parametro @pStatoNotifica indica quali notifiche si volgiono in output e può assumere uno di questi valori:
- 0 = Tutte
- 1 = SoloAttive (non dismesse) ==> DEFAULT
- 2 = SoloNonLette (DataDismissione NULL + DataPrimaVisualizzazione NULL)
*/
CREATE PROCEDURE [dbo].[Notifiche_Lista]
	@pUserId			VARCHAR(100),
	@pIdCliente			INT = NULL,
	@pStatoNotifica		TINYINT = 1,
	@pPageNumber		INT = 1,
	@pPageSize			INT = 10
AS
BEGIN
	SELECT	n.*
	FROM vNotifiche n
	WHERE (@pUserId = n.UserIdNotifiche) AND
		(@pIdCliente IS NULL OR @pIdCliente = n.IdClienteNotifiche) AND
		(((COALESCE(@pStatoNotifica, 1) =1) AND (n.DataDismissioneNotifiche IS NULL) AND
				(COALESCE(n.DataFineVisibilitaNotifiche, SYSDATETIME()) >= SYSDATETIME())) 
		   OR ((@pStatoNotifica = 2) AND  
				(n.DataDismissioneNotifiche IS NULL) AND (n.DataPrimaVisualizzazioneNotifiche IS NULL) AND
				(COALESCE(n.DataFineVisibilitaNotifiche, SYSDATETIME()) >= SYSDATETIME())
		   ) 
		   OR (@pStatoNotifica = 0))
	ORDER BY COALESCE(DataInizioVisibilitaNotifiche, DataCreazioneNotifiche) DESC
	OFFSET @pPageSize * (@pPageNumber - 1) ROWS
    FETCH NEXT @pPageSize ROWS ONLY OPTION (RECOMPILE);
END