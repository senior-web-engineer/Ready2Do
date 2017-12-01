set identity_insert TipologieImmagini ON

INSERT INTO TipologieImmagini (Id, Codice, Nome, Descrizione)
	VALUES (1, 'LOGO', 'Logo Cliente', 'Logo Cliente'),
		   (2, 'SFONDO', 'Sfondo Home Page', 'Sfondo Home Page Cliente'),
		   (3, 'GALLERY', 'Gallery Cliente', 'Gallery Cliente')

set identity_insert TipologieImmagini off