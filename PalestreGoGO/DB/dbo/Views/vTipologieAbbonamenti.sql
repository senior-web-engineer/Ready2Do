CREATE VIEW [dbo].[vTipologieAbbonamenti]
AS
	SELECT	Id				AS IdTipologieAbbonamenti
			,IdCliente		AS IdClienteTipologieAbbonamenti
			,Nome			AS NomeTipologieAbbonamenti
			,DurataMesi		AS DurataMesiTipologieAbbonamenti
			,NumIngressi		AS NumIngressiTipologieAbbonamenti
			,Costo			AS CostoTipologieAbbonamenti
			,MaxLivCorsi		AS MaxLivCorsiTipologieAbbonamenti
	FROM [TipologieAbbonamenti]