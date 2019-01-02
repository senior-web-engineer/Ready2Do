INSERT INTO [StatiCliente](IdStato, Nome, Descrizione) 
VALUES (0, 'NotProvisioned', 'Cliente appena creato, provisioning non ancora completato'),
	   (3, 'Provisioned', 'Cliente creato e provisioned completato ma email non ancora confermata'),
	   (10, 'Confirmed', 'Cliente creato ed email confermata')
