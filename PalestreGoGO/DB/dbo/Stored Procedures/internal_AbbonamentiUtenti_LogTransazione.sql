/*
Traccia le variazioni (incrementi e decrementi) agli abbonamenti degli utenti.
Le tipologie di transazioni sono:
- APP: Registrazione di un appuntamento da parte dell'utente ==> DECREMENTO
- WLI: Iscrizione ad una wait list ==> DECREMENTO
- WLD: Cancellazione da una Wait list ==> INCREMENTO
- CAP: Cancellazione appuntamenti ==> INCREMENTO
- EDT: Edit manuale dell'abbonamento ==> INCREMENTO o DECREMENTO dipende
*/
CREATE PROCEDURE [dbo].[internal_AbbonamentiUtenti_LogTransazione]
	@pIdAbbonamento			int = 0,
	@pTipoTrans				CHAR(3),
	@pQuantita				int = 1,
	@pDataOperazione		datetime2,
	@pIdAppuntamento		INT = NULL,
	@pIdWL					INT = NULL
AS
BEGIN
	DECLARE @payload NVARCHAR(500) = NULL
	SET @pTipoTrans = UPPER(@pTipoTrans)
	SET @pDataOperazione = COALESCE(@pDataOperazione, SYSDATETIME())

	IF NOT @pTipoTrans IN ('APP', 'WLI', 'WLD', 'CAP', 'EDT')
	BEGIN
		RAISERROR(N'Tipo transazione [%s] non gestita', 16, 1, @pTipoTrans);
		RETURN -1
	END

	IF @pTipoTrans <> 'EDT' AND @pIdWL IS NULL AND @pIdAppuntamento IS NULL
	BEGIN
		RAISERROR(N'Almeno uno tra @pIdAppuntamento e @pIdWL deve essere valorizzato', 16, 0);
		RETURN -1
	END
	IF @pTipoTrans <> 'EDT'
	BEGIN
		SELECT @payload = (select @pIdAppuntamento AS IdAppuntamento,
								  @pIdWL AS IdWaitingList
							for json path)
	END

	INSERT INTO AbbonamentiTransazioni (IdAbbonamento, TipoTransazione, DataTransazione, Quantita, Payload)
		SELECT @pIdAbbonamento, @pTipoTrans, @pDataOperazione, @pQuantita, @payload						   
END