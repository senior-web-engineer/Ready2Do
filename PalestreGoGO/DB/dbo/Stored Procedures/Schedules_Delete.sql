CREATE PROCEDURE [dbo].[Schedules_Delete]
	@pId					INT,
	@pIdCliente				INT = 0
AS
BEGIN
SET XACT_ABORT ON
BEGIN TRANSACTION

	UPDATE S
		SET S.DataCancellazione = SYSDATETIME()
	FROM [Schedules] S
		WHERE S.Id = @pId
		AND S.IdCliente = @pIdCliente
		--Cancelliamo uno schedule SOLO se non ci sono appuntamenti (NON CANCELLATI)
		AND NOT EXISTS (SELECT * FROM Appuntamenti a WHERE A.ScheduleId = S.Id AND a.DataCancellazione IS NULL)

	IF @@ROWCOUNT <> 1
	BEGIN
		ROLLBACK;
		RAISERROR(N'Impossibile cancellare uno Schedule per cui esistono Appuntamenti', 16, 1);
		RETURN -1;
	END
	ELSE
	BEGIN
		COMMIT;
		RETURN 1;
	END
END