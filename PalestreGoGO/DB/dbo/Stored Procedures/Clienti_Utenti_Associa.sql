CREATE PROCEDURE [dbo].[Clienti_Utenti_Associa]
	@pIdCliente			int,
	@pIdUtente			varchar(50),
	@pNominativo		nvarchar(250),
	@pDisplayName		nvarchar(100),
	@pResult			INT OUTPUT
AS
BEGIN
	INSERT INTO ClientiUtenti(IdCliente, IdUtente, NominativoUser, UserDisplayName)
	VALUES(@pIdCliente, @pIdUtente, @pNominativo, @pDisplayName);

	SET @pResult = SCOPE_IDENTITY();
END