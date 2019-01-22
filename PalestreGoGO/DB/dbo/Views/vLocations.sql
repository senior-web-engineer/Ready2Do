CREATE VIEW [dbo].[vLocations]
AS 
	SELECT   Id					AS IdLocations
			,IdCliente			AS IdClienteLocations
			,Nome				AS NomeLocations
			,Descrizione		AS DescrizioneLocations
			,CapienzaMax		AS CapienzaMaxLocations
			,Colore				AS ColoreLocations
			,ImageUrl			AS ImageUrlLocations
			,IconUrl			AS IconUrlLocations
			,DataCreazione		AS DataCreazioneLocations
			,DataCancellazione	AS DataCancellazioneLocations
	FROM Locations
