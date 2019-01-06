CREATE PROCEDURE [dbo].[ListeAttesa_CleanExpired]
	@pMaxItems	INT = 1000
AS
BEGIN
	-- Marcare come cancellate @pMaxItems Records in WaitList scaduti
	-- Per ciscuno di essi riacccreditare l'ingresso all'owner

	DECLARE @tblUpdated TABLE(UserId VARCHAR(100), IdAbbonamento INT, IdWL INT)
BEGIN TRANSACTION
	DECLARE @dtOperazione datetime2 = SYSDATETIME()

	UPDATE T
		SET DataCancellazione = @dtOperazione ,
			CausaleCancellazione = 3
		OUTPUT inserted.UserId, inserted.IdAbbonamento, inserted.Id INTO @tblUpdated(UserId, IdAbbonamento, IdWL)
	FROM ListeAttesa T
		INNER JOIN (SELECT TOP(@pMaxItems) WITH TIES Id 
					FROM ListeAttesa 
					ORDER BY IdSchedule) L ON L.Id = T.Id

	UPDATE AU
		SET IngressiResidui = IngressiResidui +1
	FROM AbbonamentiUtenti AU
		INNER JOIN @tblUpdated T ON T.IdAbbonamento = AU.Id AND T.UserId = AU.UserId
	
	INSERT INTO AbbonamentiTransazioni (IdAbbonamento, TipoTransazione, DataTransazione, Quantita, Payload)
		SELECT IdAbbonamento, 'WLD',  @dtOperazione, 1, CONCAT(N'[{"IdWaitingList":', IdWL, '}]')
		FROM @tblUpdated
COMMIT
END