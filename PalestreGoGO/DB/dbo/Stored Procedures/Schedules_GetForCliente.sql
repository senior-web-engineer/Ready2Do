CREATE PROCEDURE [dbo].[Schedules_GetForCliente]
	@pIdCliente		INT,
	@pStartDate		DATETIME2(2) = NULL,
	@pEndDate		DATETIME2(2) = NULL,
	@pIdLocation	INT	= NULL
AS
BEGIN
	SET @pStartDate = COALESCE(@pStartDate, '1900-01-01')
	SET @pEndDate = COALESCE(@pEndDate, '9999-01-01')
	SELECT s.[Id], 
		   s.[CancellabileFinoAl], 
		   s.[DataOraInizio], 
		   s.[DataCancellazione], 
		   s.[IdCliente], 
		   s.[IdLocation], 
		   s.[IdTipoLezione], 
		   s.[Istruttore], 
		   s.[Note], 
		   s.[PostiDisponibili], 
		   s.[PostiResidui], 
		   s.[Title], 
		   s.[UserIdOwner], 
		   l.[Id] AS IdLocation, 
		   l.[CapienzaMax], 
		   l.[Descrizione], 
		   l.[IdCliente], 
		   l.[Nome], 
		   tl.[Id] AS IdTipoLezione, 
		   tl.[Descrizione], 
		   tl.[Durata], 
		   tl.[IdCliente], 
		   tl.[LimiteCancellazioneMinuti], 
		   tl.[Livello], 
		   tl.[MaxPartecipanti], 
		   tl.[Nome]
FROM [Schedules] AS s
	INNER JOIN [Locations] AS l ON s.[IdLocation] = l.[Id]
	INNER JOIN [TipologieLezioni] AS tl ON s.[IdTipoLezione] = tl.[Id]
WHERE s.IdCliente = @pIdCliente
AND DataOraInizio BETWEEN @pStartDate AND @pEndDate
AND ((@pIdLocation IS NULL) OR (s.IdLocation = @pIdLocation))
END
