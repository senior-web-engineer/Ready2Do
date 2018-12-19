/*
Cosa facciamo se rendiamo un abbonamento non più valido ma un utente con quell'abbonamento ha delle prenotazioni in essere?
==> la modifica ha effetto solo per le nuove associazioni, nel senso che non sarà più possibile associare l'abbonamento ad altri 
	utenti ma chi già ce l'ha continuerà ad averlo (diverso dalla cancellazione)

*/
CREATE PROCEDURE [dbo].[TipologieAbbonamenti_Modifica]
	@pId							INT,
	@pIdCliente						INT,
	@pNome							NVARCHAR(100),
	@pDurataMesi					SMALLINT = NULL,
	@pNumIngressi					SMALLINT = NULL,
	@pCosto							DECIMAL(10,2) = NULL,
	@pMaxLivCorsi					SMALLINT = NULL,
	@pValidoDal						DATETIME2(2) ,
	@pValidoFinoAl					DATETIME2(2) = NULL
AS
BEGIN
	UPDATE TipologieAbbonamenti
	SET Nome = @pNome,
		DurataMesi = @pDurataMesi,
		NumIngressi = @pNumIngressi,
		Costo = @pCosto,
		MaxLivCorsi = @pMaxLivCorsi,
		ValidoDal = @pValidoDal,
		ValidoFinoAl = @pValidoFinoAl
	WHERE Id = @pId
	AND IdCliente = @pIdCliente
	
	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR(N'Parametri non validi', 16, 1);
		RETURN -1;
	END
	RETURN 0
END
