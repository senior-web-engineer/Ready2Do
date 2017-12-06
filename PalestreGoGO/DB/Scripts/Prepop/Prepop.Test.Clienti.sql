/* Inserimento di alcuni Clienti per il test */
DECLARE @idOwnerCliente1	UNIQUEIDENTIFIER = '{A6688CFC-8E67-4EB8-BA2C-58C5F7DD69D1}'
DECLARE @tokenCliente1		VARCHAR(50) = '{93210397-D779-4F1A-8C70-12EF5E991263}'
DECLARE @idOwnerCliente2	UNIQUEIDENTIFIER = '{E0EA62E3-6F88-4F3E-8B42-4CFD58FFF424}'
DECLARE @tokenCliente2		VARCHAR(50) = '{3B9C594B-C81E-496C-9288-E10504D762DA}'


SET IDENTITY_INSERT [Clienti] ON 

INSERT INTO [Clienti] (Id, Nome, RagioneSociale, Email, NumTelefono, Descrizione, IdTipologia, Indirizzo, Citta, ZipOrPostalCode, Country, 
						Latitudine, Longitudine, DataCreazione, IdUserOwner, ProvisioningToken, DataProvisioning, UrlRoute, OrarioApertura)
VALUES (1, 'Cliente Test 1', 'Cliente 1 - RagioneSociale Sociale', 'cliente1@test.tst', '06000010101', 'Cliente di test 1', 0, 'Via Trionfale 1000', 'Roma',
						'00135', 'Italy', 0.0001, 0.0002, GETDATE(), @idOwnerCliente1, @tokenCliente1,  GETDATE(), 'cliente1', 'Sempre aperto'),
	   (2, 'Cliente Test 2', 'Cliente 2 - RagioneSociale Sociale', 'cliente2@test.tst', '062222222', 'Cliente di test 2', 0, 'Via Cassia 2345', 'Roma',
						'00172', 'Italy', 0.9001, 0.9002, GETDATE(), @idOwnerCliente2, @tokenCliente2,  GETDATE(), 'cliente2', 'Sempre chiuso')

SET IDENTITY_INSERT [Clienti] OFF


SET IDENTITY_INSERT [ClientiImmagini] ON 
INSERT INTO [ClientiImmagini](Id, IdCliente, IdTipoImmagine, Nome, Url, Descrizione)
VALUES	(1, 1, 1, 'Chiappe', 'https://i2.wp.com/eccellentedonna.it/wp-content/uploads/2017/01/Esercizi-efficaci-per-rassodare-i-glutei.jpg', 'Desc Chiappe'),
		(2, 1, 2, 'Chiappe', 'https://i2.wp.com/eccellentedonna.it/wp-content/uploads/2017/01/Esercizi-efficaci-per-rassodare-i-glutei.jpg', 'Desc Chiappe'),
		(3, 2, 1, 'Front', 'https://aziende.axelero.it/hs-fs/hubfs/tonino-ferro-theme/TOFU/come-promuovere-una-palestra-eventi.jpg?t=1511542189986&width=1120&name=come-promuovere-una-palestra-eventi.jpg', 'Promozione Palestra'),
		(4, 2, 2, 'Front', 'https://aziende.axelero.it/hs-fs/hubfs/tonino-ferro-theme/TOFU/come-promuovere-una-palestra-eventi.jpg?t=1511542189986&width=1120&name=come-promuovere-una-palestra-eventi.jpg', 'Promozione Palestra')
SET IDENTITY_INSERT [ClientiImmagini] OFF 

INSERT INTO [ClientiMetadati] (IdCliente, [Key], [Value])
		VALUES (1, 'Tag1','Tag1'),
			   (1, 'Tag2','Tag2'),
			   (2, 'Tag3','Tag3'),
			   (2, 'Tag4','Tag4')


SET IDENTITY_INSERT [TipologieLezioni]  ON 
INSERT INTO [dbo].[TipologieLezioni] ([Id], [IdCliente], [Nome], [Descrizione], [Durata], [MaxPartecipanti], [LimiteCancellazioneMinuti], [Livello])
    VALUES(1, 1, 'Yoga', 'Yoga per tutti', 50, 10, 120, 150),
		  (2, 1, 'Pilates', 'Pilates per tutti', 50, 15, 120, 100),
		  (3, 2, 'Zumba', 'Zumba per tutti', 50, 10, 120, 150),
		  (4, 2, 'Funzionale', 'Funzionale per tutti', 50, 15, 120, 100)
SET IDENTITY_INSERT [TipologieLezioni]  OFF


SET IDENTITY_INSERT [TipologieAbbonamenti]  ON 
INSERT INTO [dbo].[TipologieAbbonamenti]([Id], [IdCliente], [Nome], [DurataMesi], [NumIngressi], [Costo],[MaxLivCorsi])
    VALUES(1, 1, 'Trimestrale (Liv. 200)', 3, 100, 120, 200),
		  (2, 1, '20 Ingressi', 9999, 20, 200, 1000),
		  (3, 2, 'Mensile (Liv. 1000)', 1, 100, 75, 1000)
SET IDENTITY_INSERT [TipologieAbbonamenti]  OFF


SET IDENTITY_INSERT [Locations]  ON 
INSERT INTO [dbo].[Locations]([Id], [IdCliente], [Nome], [Descrizione], [CapienzaMax])
    VALUES(1, 1, 'Sala Blu', 'Desc Sala Blu', 25),
		  (2, 1, 'Sala Rossa', 'Desv Sala Rossa', 50),
		  (3, 2, 'Sala YOGA', 'Sala per corsi di Yoga', 30)
SET IDENTITY_INSERT [Locations]  OFF