/*
	Verifica se il nome passato esiste già per il cliente (tra quelli non cancellati)
	Ritorna (nel parametro di Out):
	1: Nome non trovato
	0: Nome già in uso
*/
CREATE PROCEDURE [dbo].[TipologieLezioni_CheckNome]
	@pIdCliente			int,
	@pNomeTipoLezione	NVARCHAR(100),
	@pId				int = NULL,
	@pResult			BIT OUT
AS
BEGIN
	IF EXISTS(SELECT * FROM TipologieLezioni WHERE IdCliente = @pIdCliente AND Nome = @pNomeTipoLezione AND DataCancellazione IS NULL AND ((@pId IS NULL) OR (Id <> @pId)))
		SET @pResult = 0
	ELSE
		SET @pResult = 1;
END