/*
	Verifica se il nome passato esiste già per il cliente (tra i record non cancellati).
	Ritorna (nel parametro di Out):
	1: Nome non trovato
	0: Nome già in uso
*/
CREATE PROCEDURE [dbo].[TipologieAbbonamenti_CheckNome]
	@pIdCliente				int,
	@pNomeTipoAbbonamento	NVARCHAR(100),
	@pResult				BIT OUT
AS
BEGIN
	IF EXISTS(SELECT * FROM TipologieAbbonamenti WHERE IdCliente = @pIdCliente AND Nome = @pNomeTipoAbbonamento AND DataCancellazione IS NULl)
		SET @pResult = 0
	ELSE
		SET @pResult = 1;
END