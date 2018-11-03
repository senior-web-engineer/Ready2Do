CREATE PROCEDURE [dbo].[Clienti_OrarioSave]
	@pIdCliente			int,
	@pOrarioApertura	varchar(max)
AS
BEGIN
	UPDATE  Clienti
		SET OrarioApertura = @pOrarioApertura
	WHERE Id = @pIdCliente;
END