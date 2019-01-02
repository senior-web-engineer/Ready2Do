/*
Al momento la modifica di una tipologia lezione non impatta sulle prenotazioni già in essere.
ATTENZIONE:
1) se si cambia il prezzo di una lezione dopo che utente ha già prenotato, l'eventuale annullamento della prenotazione rimborserebbe il nuovo prezzo!
   FIX: Non consentiamo modifiche al prezzo se esistono prenotazioni in essere (in futuro si dovrà gestire il prezzo della lezione in modo più strutturato con 
NOTE:
 non facciamo controlli sul valore di @pMaxPartecipanti
*/
CREATE PROCEDURE [dbo].[TipologieLezioni_Modifica]
	@pId							INT,
	@pIdCliente						INT,
	@pNome							NVARCHAR(100),
	@pDescrizione					NVARCHAR(500) = NULL,
	@pDurata						INT,
	@pMaxPartecipanti				INT = NULL,
	@pLimiteCancellazioneMinuti		SMALLINT = NULL,
	@pLivello						SMALLINT = 0,
	@pPrezzo						DECIMAL(10,2) = NULL
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dtOperazione DATETIME2 = SYSDATETIME();
	DECLARE @logMsg NVARCHAr(2000) = ''
	SET @logMsg = CONCAT('BEGIN ', OBJECT_NAME(@@PROCID), '(@pId=', @pId, ', @pIdCliente=', @pIdCliente, ', @pNome=',@pNome, ', @pDescrizione=', @pDescrizione,
					     ', @pDurata=', @pDurata, ', @pMaxPartecipanti=', @pMaxPartecipanti,', @pLimiteCancellazioneMinuti=',@pLimiteCancellazioneMinuti,
						 ', @pLivello=',@pLivello,', @pPrezzo=', @pPrezzo, ')')
	exec [dbo].[internal_LogMessage] @pIdCliente, NULL, 'L', @logMsg, @dtOperazione
	
	UPDATE TipologieLezioni
		SET IdCliente = @pIdCliente, 
			Nome = @pNome, 
			Descrizione = @pDescrizione, 
			Durata = @pDurata, 
			MaxPartecipanti = @pMaxPartecipanti, 
			LimiteCancellazioneMinuti = @pLimiteCancellazioneMinuti, 
			Livello = @pLivello,
			Prezzo = @pPrezzo
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	--Il prezzo può essere variato SOLO se non esistono prenotazioni attive
	AND (((Prezzo IS NULL AND @pPrezzo IS NULL) OR Prezzo = @pPrezzo) OR 
		 NOT EXISTS(
			SELECT 1 FROM [Appuntamenti] a WITH (NOLOCK)
				INNER JOIN [Schedules] s WITH (NOLOCK) ON a.ScheduleId = s.Id 
			WHERE a.IdCliente = @pIdCliente
			AND a.DataCancellazione IS NULL
			AND s.IdCliente = @pIdCliente
			AND s.DataCancellazione IS NULL
			AND s.DataOraInizio > @dtOperazione
		 ))
	AND DataCancellazione IS NULL

	IF @@ROWCOUNT = 0
	BEGIN
		set @logMsg = CONCAT(OBJECT_NAME(@@PROCID),'Impossibile eseguire l''aggiornamento perché i parametri passati non sono validi oppure si sta cercando di aggiornare il prezzo ma esiste una prenotazione attiva associata')
		exec [dbo].[internal_LogMessage] @pIdCliente, NULL,'E', @logMsg
		RAISERROR('Parametri non validi!', 16, 0);
		RETURN -1;
	END
	
	set @logMsg = CONCAT('END ', OBJECT_NAME(@@PROCID))
	exec [dbo].[internal_LogMessage] @pIdCliente, NULL,'V', @logMsg
	
	RETURN 0
END
