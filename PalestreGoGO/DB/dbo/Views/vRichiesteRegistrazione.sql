CREATE VIEW [dbo].[vRichiesteRegistrazione]
AS 
SELECT	 Id				AS IdRichiesteRegistrazione
		,DataRichiesta	AS DataRichiestaRichiesteRegistrazione
		,CorrelationId	AS CorrelationIdRichiesteRegistrazione
		,UserCode		AS UserCodeRichiesteRegistrazione
		,Username		AS UsernameRichiesteRegistrazione
		,Expiration		AS ExpirationRichiesteRegistrazione
		,DataConferma	AS DataConfermaRichiesteRegistrazione
FROM RichiesteRegistrazione