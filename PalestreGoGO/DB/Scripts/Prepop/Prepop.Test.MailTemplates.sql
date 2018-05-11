INSERT INTO MailTemplates([TipoMail], [Subject], [HtmlBody], [TextBody])
	VALUES (1, 'Conferma Cliente', '<html><body><h1>Per attivare il tuo account clicca il seguente link <a href="{{UrlAttivazione}}?code={{Code}}&email={{email}}">Attiva Account</a></h1></body></html>', 
				'Visita il seguente indirizzo per attivare il tuo account {{UrlAttivazione}}?code={{Code}}&email={{email}}')

INSERT INTO MailTemplates([TipoMail], [Subject], [HtmlBody], [TextBody])
	VALUES (2, 'Conferma Utente', '<html><body><h1>Per attivare il tuo account clicca il seguente link <a href="{{UrlAttivazione}}?code={{Code}}&email={{email}}">Attiva Account</a></h1></body></html>',
				'Visita il seguente indirizzo per attivare il tuo account {{UrlAttivazione}}?code={{Code}}&email={{email}}')