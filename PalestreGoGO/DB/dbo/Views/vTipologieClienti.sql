CREATE VIEW [dbo].[vTipologieClienti]
AS 
	SELECT   Id				AS IdTipologieClienti
			,Nome			AS NomeTipologieClienti
			,Descrizione	AS DescrizioneTipologieClienti
	FROM TipologieClienti
