CREATE PROCEDURE [dbo].[AtualizarStatusDatas]
	@DataId INT,
	@Status TINYINT
AS
BEGIN
	UPDATE
		PresencaDatas
	SET 
		[Status] = @Status
		, DataHoraRegistro = CASE WHEN @Status = 2 THEN GETDATE() ELSE NULL END 
	WHERE
		Id = @DataId 

	DECLARE @PresencaId INT
	SELECT
		@PresencaId = PresencaId
	FROM	
		PresencaDatas
	WHERE 
		Id = @DataId 

	UPDATE
		Presenca
	SET
		[Status] = CASE WHEN NOT EXISTS(SELECT TOP 1 1 FROM PresencaDatas WHERE PresencaId = @PresencaId AND [Status] <= 2) THEN 3 ELSE 1 END
	WHERE
		Id = @PresencaId
END
