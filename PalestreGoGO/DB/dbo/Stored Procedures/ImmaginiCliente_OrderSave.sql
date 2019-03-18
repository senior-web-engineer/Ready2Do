CREATE PROCEDURE [dbo].[ImmaginiCliente_OrderSave]
	@pIdCliente			INT,
	@pOrdinamento		[dbo].[udtListOfIntWithOrder] READONLY
AS
BEGIN
	UPDATE CI
		SET Ordinamento = ORD.[Order]
		FROM ClientiImmagini CI
			INNER JOIN @pOrdinamento ORD ON ORD.Id = CI.Id AND CI.IdCliente = @pIdCliente
	WHERE IdCliente = @pIdCliente

END