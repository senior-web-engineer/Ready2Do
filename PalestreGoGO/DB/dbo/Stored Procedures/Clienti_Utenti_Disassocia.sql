/*
Cancella l'associazione tra un utente ed un cliente.
Cosa succede alle entità esistenti per l'utente per il cliente?
Per ora le lasciamo inalterate, eventuali prenotazioni ed abbonamenti restano attivi 
anche se l'utente non potrà più accedervi.
*/
CREATE PROCEDURE [dbo].[Clienti_Utenti_Disassocia]
	@pIdCliente			int,
	@pIdUtente			varchar(50)
AS
BEGIN
	UPDATE ClientiUtenti
		SET DataCancellazione = SYSDATETIME()
	WHERE IdCliente = @pIdCliente
	AND IdUtente = @pIdUtente
	AND DataCancellazione IS NULL


END