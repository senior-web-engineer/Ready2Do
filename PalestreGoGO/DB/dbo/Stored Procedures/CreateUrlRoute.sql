/*
Accetta in input il NomeCliente opportunamete Url Encoded dal chiamante,
ne garantisce l'univocità aggiungendo eventualmente un contatore in coda
*/
create procedure CreateUrlRoute
	@pNomeClienteUrlEncoded varchar(200)
AS
BEGIN
	if @pNomeClienteUrlEncoded IS NULL
	BEGIN
		RAISERROR (N'NomeCliente richiesto e non specificato', 16,0);
		RETURN -1;
	END

	DECLARE @result			VARCHAR(205),
			@existing		VARCHAR(205),
			@idxDelimiter	INT,
			@counter		INT;

	SELECT TOP 1 @existing = UrlRoute 
	FROM [Clienti] 
	WHERE UrlRoute = @pNomeClienteUrlEncoded
	
	IF @existing IS NOT NULL
	BEGIN
		--Se esiste già un cliente con lo stesso UrlRoute, andiamo a vedere se ne esistono anche altri con 
		-- il progressivo e prendiamo quello con il progressivo più alto
		SELECT TOP 1 @existing = COALESCE(UrlRoute, @existing)
		FROM [Clienti] 
		WHERE UrlRoute LIKE @pNomeClienteUrlEncoded + '!_!_'+'[0-9][0-9][0-9]' ESCAPE '!'	
		ORDER BY UrlRoute DESC --così prendiamo quello con il progressivo più alto
	END
	--ATTENZIONE: assumiamo che non esista un sequenza di 2 underscore consecutivi che non siano quelli utilizzati 
	-- per seprare il nome dal progressivo
	-- Per avere questa garanzia è opportuno rimpiazzarli prima di produrre il risultato
	SET @result = REPLACE(REPLACE(@pNomeClienteUrlEncoded, '__', '_'), '%5F%5F','%5F')
	IF @existing IS NOT NULL
	BEGIN		
		SET @idxDelimiter = CHARINDEX('__', @existing, 0);
		IF @idxDelimiter > 0
		BEGIN
			SET @counter = CAST (SUBSTRING(@existing, @idxDelimiter +2, LEN(@existing) - 1 - @idxDelimiter) AS INT) +1
		END
		ELSE
		BEGIN
			SET @counter = 1
		END
		SET @result = @result + '__'+ RIGHT('000' + CAST(@counter AS VARCHAR(3)), 3)
	END

	SELECT @result;
END