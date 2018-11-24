CREATE VIEW [dbo].[vLocations]
AS 
	SELECT   Id				AS IdLocations
			,IdCliente		AS IdClienteLocations
			,Nome			AS NomeLocations
			,Descrizione	AS DescrizioneLocations
			,CapienzaMax	AS CapienzaMaxLocations
	FROM Locations
