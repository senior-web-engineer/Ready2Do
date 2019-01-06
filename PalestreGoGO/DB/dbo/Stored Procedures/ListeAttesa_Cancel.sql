/*
Annulla l'iscrizione di un utente ad una lista di attesa
*/
CREATE PROCEDURE [dbo].[ListeAttesa_Cancel]
	@pIdCliente		INT, 
	@pUserId		VARCHAR(100),
	@pId			INT,	
	@pEsito			INT
AS
BEGIN
BEGIN TRANSACTION
	DECLARE @tblUpdated TABLE (IdAbbonamento INT, DataOperazione DATETIME2)
	DECLARE @idAbbonamento INT, @dataOperazione DATETIME2

	UPDATE ListeAttesa
		SET DataCancellazione = SYSDATETIME()
		OUTPUT inserted.IdAbbonamento, inserted.DataCancellazione INTO @tblUpdated(IdAbbonamento, DataOperazione)
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	AND UserId = @pUserId
	AND	DataCancellazione IS NULL -- Non deve già essere cancellata
	AND DataConversione IS NULL -- Se convertita in appuntamento non è cancellabile
	
	SELECT  @idAbbonamento = IdAbbonamento,
			@dataOperazione = DataOperazione
	FROM @tblUpdated

	--Riaccreditiamo l'ingresso sull'abbonamento utilizzato
	UPDATE AbbonamentiUtenti
		SET IngressiResidui = IngressiResidui +1
	WHERE Id = @idAbbonamento

	-- Tracciamo la transazione
	EXEC [dbo].[internal_AbbonamentiUtenti_LogTransazione] @idAbbonamento, 'WLD', 1, @dataOperazione, NULL, @pId

COMMIT
END