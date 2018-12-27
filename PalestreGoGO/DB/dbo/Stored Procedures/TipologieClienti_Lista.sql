CREATE PROCEDURE [dbo].[TipologieClienti_Lista]
	@pIncludeDeleted	BIT = 0
AS
BEGIN
	SELECT	Id,
			Nome,
			Descrizione,
			DataCreazione,
			DataCancellazione
	FROM TipologieClienti
	WHERE ((@pIncludeDeleted IS NULL) OR (DataCancellazione IS NULL))

	RETURN 0
END
