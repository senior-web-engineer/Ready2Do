CREATE FUNCTION [dbo].[ExistsAbbonamentoValido]
(
	@pIdCliente			int,
	@pUserId			varchar(100),
	@pLivelloLezione	smallint
)
RETURNS TABLE AS RETURN
(
	SELECT TOP 1 CAST(CASE WHEN au.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS HasAbbonamento
	FROM AbbonamentiUtenti au WITH (NOLOCK)
		INNER JOIN TipologieAbbonamenti ta ON au.IdTipoAbbonamento = ta.Id
	WHERE au.IdCliente = @pIdCliente 
	AND au.UserId =@pUserId
	AND au.DataInizioValidita < SYSDATETIME()
	AND COALESCE(au.IngressiResidui,1)> 0
	AND au.DataCancellazione IS NULL
	AND au.Scadenza >= SYSDATETIME()
	AND COALESCE(ta.MaxLivCorsi, 32767) >= COALESCE(@pLivelloLezione,0) 
)
