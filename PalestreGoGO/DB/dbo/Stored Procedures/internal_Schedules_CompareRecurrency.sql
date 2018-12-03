/*
Confronta due strutture JSON contenente le definizioni di Ricorrenze di uno Schedule.
Consideriamo @pRecurrency1 = A come il vecchio e @pRecurrency2 = B come il nuovo per convenzione
ATTENZIONE! Devono essere entrambi NOT NULL altrimenti non possiamo confrontarli
RITORNA:
 - in @pResult: 1 se sono uguali, 0 se sono diverse
 - in @pDifferences i campi modificati come bitfield con la seguente semantica:
	None		= 0
	Recurrency	= 1 << 1
	RepeatUntil = 1 << 2
	DaysOfWeek  = 1 << 3 

** DaysOfWeek**  possono cambiare nei seguenti modi:
	- Presenti in A e non in B ==> "elimiminati" ==> recurrency potrebbe non essere più weekly
	- Presenti in B e non in A ==> "aggiunti" ==> ATTENZIONE CHE Recurrency deve essere weekly
	- Presenti sia in A che in B ==> "modificati" ==> sono cambiati i singoli giorni, abbiamo 2 casi:
					# cambio di cardinalità (Es: da 1 a 2 giorni la settimana [Lun] => [Lun, Mer] o viceversa [Lun, Mar] => [Lun])
					# stessa cardinalità  ma giorni diversi (Es: Da [Lun, Mer, Ven] => [Lun, Mar, Sab])
*/
CREATE PROCEDURE [dbo].[internal_Schedules_CompareRecurrency]
	@pRecurrency1	NVARCHAR(MAX),
	@pRecurrency2	NVARCHAR(MAX),
	@pResult		BIT	OUT,
	@pDifferences	INT OUT
AS
BEGIN
	DECLARE @recurrency VARCHAR(100),
			@endData DATE,
			@repeatFor INT,
			@firstDay VARCHAR(100)
	DECLARE @tblDayR1 TABLE([DayOfWeek] VARCHAR(100))
	DECLARE @tblDayR2 TABLE([DayOfWeek] VARCHAR(100))

	DECLARE @CONST_NO_CHANGES		  INT  = 0x000,
			@CONST_RECURRENCY_CHANGED INT  = 0x001,
			@CONST_REPEATUNTIL_CHANGED INT = 0x002,
			@CONST_DAYSOFWEEK_CHANGED INT  = 0x004

	SELECT @pResult = NULL, @pDifferences = @CONST_NO_CHANGES;

	IF @pRecurrency1 IS NULL OR @pRecurrency2 IS NULL 
	BEGIN 
		SELECT @pResult = NULL, @pDifferences = NULL;
		RAISERROR('I parametri non possono essere NULL', 16 ,0);
		RETURN -1;
	END
	-- Se non sono JSON è un errore
	IF ISJSON(@pRecurrency1) = 0 OR ISJSON(@pRecurrency2) = 0
	BEGIN
		SELECT @pResult = NULL, @pDifferences = NULL;
		RAISERROR('I parametri devono essere dei JSON validi', 16 ,0);
		RETURN -2;
	END

	-- Andiamo a confrontar le varie componenti scalari 
	IF LOWER(JSON_VALUE(@pRecurrency1, '$.Recurrency')) <> LOWER(JSON_VALUE(@pRecurrency2, '$.Recurrency')) 
	BEGIN
		SET @pResult = 0
		SET @pDifferences |= @CONST_RECURRENCY_CHANGED
	END
	
	IF COALESCE(LOWER(JSON_VALUE(@pRecurrency1, '$.RepeatUntil')), '') <> COALESCE(LOWER(JSON_VALUE(@pRecurrency2, '$.RepeatUntil')), '')
	BEGIN
		SET @pResult = 0
		SET @pDifferences |= @CONST_REPEATUNTIL_CHANGED
	END
	IF COALESCE(LOWER(JSON_VALUE(@pRecurrency1, '$.RepeatFor')), '') <> COALESCE(LOWER(JSON_VALUE(@pRecurrency2, '$.RepeatFor')), '')
	BEGIN
		SET @pResult = 0
		SET @pDifferences |= @CONST_REPEATUNTIL_CHANGED
	END
	-- Verifichiamo se i DaysOfWeek sono gli stessi
	IF EXISTS( SELECT * FROM
						  (SELECT lower([DayOfWeek]) AS [DayOfWeek]
									FROM OPENJSON( @pRecurrency1, '$.DaysOfWeek' ) 
									WITH ([DayOfWeek] NVARCHAR(25) '$')) R1
				FULL JOIN (SELECT LOWER([DayOfWeek]) AS [DayOfWeek]
									FROM OPENJSON( @pRecurrency2, '$.DaysOfWeek' ) 
									WITH ([DayOfWeek] NVARCHAR(25) '$')) R2 ON R1.[DayOfWeek] = R2.[DayOfWeek]
				WHERE R1.[DayOfWeek] IS NULL OR R2.[DayOfWeek] IS NULL)
	BEGIN
		SET @pResult = 0
		SET @pDifferences |= @CONST_DAYSOFWEEK_CHANGED
	END

	--Se è ancora NULL a questo punto vuol dire che i 2 JSON sono uguali
	IF @pResult IS NULL SET @pResult = 1
END 