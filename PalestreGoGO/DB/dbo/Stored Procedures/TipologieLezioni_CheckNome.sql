/*
	Verifica se il nome passato esiste già per il cliente.
	Ritorna (nel parametro di Out):
	1: Nome non trovato
	0: Nome già in uso
*/
CREATE PROCEDURE [dbo].[TipologieLezioni_CheckNome]
	@pIdCliente			int,
	@pNomeTipoLezione	NVARCHAR(100),
	@pResult			BIT OUT
AS
BEGIN
	IF EXISTS(SELECT * FROM TipologieLezioni WHERE IdCliente = @pIdCliente AND Nome = @pNomeTipoLezione)
		SET @pResult = 0
	ELSE
		SET @pResult = 1;
END