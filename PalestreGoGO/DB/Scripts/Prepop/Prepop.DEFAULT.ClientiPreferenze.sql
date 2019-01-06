/*
Qeusto script è solo un template (promemeoria) per la valorizzazione delle preferenze di default per un cliente al momento del provisioning
*/
DECLARE @idCliente INT = -1

INSERT INTO ClientiPreferenze ([IdCliente], [Key], [Value])
	VALUES	 (@idCliente, 'APPUNTAMENTIDACONFERMARE.EXPIRATION.WINDOW.MINUTES','2880')
			,(@idCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_DATAORA.ISCRITTI', '1') -- Indica se includere gli iscritti nell'evento di notifica per il cambio della data ed ora di uno schedule
			,(@idCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_DATAORA.WAITLIST', '1') -- Indica se includere gli iscritti alla WAITINGGLIST nell'evento di notifica per il cambio della data ed ora di uno schedule,
			,(@idCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_LOCATION.ISCRITTI', '1') -- Indica se includere gli iscritti nell'evento di notifica per il cambio della locationdi uno schedule
			,(@idCliente, 'NOTIFICHE.SCHEDULE.ONCHANGE_LOCATION.WAITLIST', '1') -- Indica se includere gli iscritti alla WAITINGGLIST nell'evento di notifica per il cambio della location di uno schedule,
