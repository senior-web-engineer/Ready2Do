CREATE PROCEDURE [dbo].[Notifiche_Add]
	@pIdTipo		INT,
	@pIdUtente		VARCHAR(100),
	@pIdCliente		INT = NULL,
	@pTitolo		NVARCHAR(50),
	@pTesto			NVARCHAR(250),
	@pIdNotitifica	BIGINT OUTPUT
AS
BEGIN
	INSERT INTO Notifiche(IdTipo, IdUtente, IdCliente, Titolo, Testo)
		VALUES (@pIdTipo, @pIdUtente, @pIdCliente, @pTitolo, @pTesto);

	SET @pIdNotitifica = SCOPE_IDENTITY();
END