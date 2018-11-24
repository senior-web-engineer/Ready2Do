CREATE VIEW [dbo].[vTipologieLezioni]
AS 
	SELECT
		 Id							AS IdTipologieLezioni
		,IdCliente					AS IdClienteTipologieLezioni
		,Nome						AS NomeTipologieLezioni
		,Descrizione				AS DescrizioneTipologieLezioni
		,Durata						AS DurataTipologieLezioni
		,MaxPartecipanti			AS MaxPartecipantiTipologieLezioni
		,LimiteCancellazioneMinuti	AS LimiteCancellazioneMinutiTipologieLezioni
		,Livello					AS LivelloTipologieLezioni
		,DataCancellazione			AS DataCancellazioneTipologieLezioni
		,DataCreazione				AS DataCreazioneTipologieLezioni
	FROM TipologieLezioni
