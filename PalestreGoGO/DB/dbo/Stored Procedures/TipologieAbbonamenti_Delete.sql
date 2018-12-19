/*
La cancellazione di una Tipologia di Abbonamento non ha impatti sugli abbonamenti già in essere che continuano ad essere validi fino a scadenza
*/
CREATE PROCEDURE [dbo].[TipologieAbbonamenti_Delete]
	@pId							INT,
	@pIdCliente						INT
AS
BEGIN
	UPDATE TipologieAbbonamenti
	SET DataCancellazione = SYSDATETIME()
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	
	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR(N'Parametri non validi', 16, 1);
		RETURN -1;
	END
	RETURN 0
END
