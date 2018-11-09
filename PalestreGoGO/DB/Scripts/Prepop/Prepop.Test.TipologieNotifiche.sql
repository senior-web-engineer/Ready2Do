SET IDENTITY_INSERT  TipologieNotifiche ON
insert into TipologieNotifiche(Id, Code, UserDismissable, AutoDismissAfter, [Priority])
	values (1, 'ACCOUNT_TO_CONFIRM', 0, NULL, 100),
		   (2, 'NORMAL_NOTIFICATION', 1, 172800, 10) --dismissable after 2 days

SET IDENTITY_INSERT  TipologieNotifiche OFF