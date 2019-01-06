CREATE PROCEDURE [dbo].[internal_ListeAttesa_PromuoviToAppuntamento]
	@pNumRecordMaxToPromote	INT = 1,
	@pIdSchedule			INT,
	@pNumRecordUpdated		INT OUTPUT
AS
BEGIN
	IF @@TRANCOUNT = 0
	BEGIN
		RAISERROR(N'Transazione richiesta!', 16, 0)
		RETURN -1;
	END
	DECLARE @idAppuntamento	INT

	DECLARE @userId			VARCHAR(100),
			@idAbbonamento	INT,
			@idSchedule		INT,
			@idCliente		INT,
			@idWaitList		INT
	
	DECLARE @dtOperazione	DATETIME2 = SYSDATETIME();

	DECLARE @tblUserWL TABLE(UserId				VARCHAR(100) NOT NULL, 
							 IdAbbonamento		INT NOT NULL, 
							 IdSchedule			INT NOT NULL,
							 IdCliente			INT NOT NULL,
							 IdWaitList			INT NOT NULL);
	UPDATE ListeAttesa
	SET DataConversione = SYSDATETIME()
		OUTPUT inserted.UserId, inserted.IdSchedule, inserted.IdAbbonamento, inserted.IdCliente, inserted.Id
			INTO @tblUserWL(UserId, IdSchedule, IdAbbonamento, IdCliente, IdWaitList)
	WHERE Id = (SELECT TOP(@pNumRecordMaxToPromote) Id 
				FROM ListeAttesa LA
				WHERE IdSchedule = @pIdSchedule
				AND DataCancellazione IS NULL
				ORDER BY DataCreazione ASC)
	SET @pNumRecordUpdated = @@ROWCOUNT

	DECLARE CUR_Records CURSOR FOR SELECT UserId, IdAbbonamento, IdSchedule, IdCliente, IdWaitList FROM @tblUserWL
	OPEN CUR_Records
	FETCH NEXT FROM CUR_Records INTO @userId, @idAbbonamento, @idSchedule, @idCliente, @idWaitList
	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Creiamo l'appuntamento per l'utente
		EXEC Appuntamenti_Add @idCliente, @userId, @idSchedule, @idAbbonamento, NULL, NULL, NULL, @idAppuntamento OUT
		-- Generiamo un evento per la notifica della trasformazione WaitList -> Appuntamento per ogni promozione
		EXEC [dbo].[internal_WaitList_NotifyPromotion] @idCliente, @idWaitList, @pIdSchedule, @idAppuntamento, @userId,  @idAbbonamento

		FETCH NEXT FROM CUR_Records INTO @userId, @idAbbonamento, @idSchedule, @idCliente, @idWaitList
	END

	CLOSE CUR_Records
	DEALLOCATE CUR_Records;
	
	RETURN 01;
END