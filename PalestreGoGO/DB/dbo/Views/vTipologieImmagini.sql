CREATE VIEW [dbo].[vTipologieImmagini]
AS 
	SELECT   Id				AS IdTipologieImmagini
			,Codice			AS CodiceTipologieImmagini
			,Nome			AS NomeTipologieImmagini
			,Descrizione	AS DescrizioneTipologieImmagini
			,DataCreazione	AS DataCreazioneTipologieImmagini
	FROM TipologieImmagini
