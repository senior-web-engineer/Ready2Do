CREATE VIEW [dbo].[vMailTemplates]
AS 
SELECT   Id					AS IdMailTemplates
		,TipoMail			AS TipoMailMailTemplates
		,[Subject]			AS SubjectMailTemplates
		,TextBody			AS TextBodyMailTemplates
		,HtmlBody			AS HtmlBodyMailTemplates
		,OnlyText			AS OnlyTextMailTemplates
		,DataCancellazione	AS DataCancellazioneMailTemplates
FROM MailTemplates
