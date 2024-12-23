CREATE PROCEDURE [dbo].[DeletePresencaDatas]
	@DataId INT
AS
BEGIN
	IF OBJECT_ID('TEMPDB..#EVENTOS') IS NOT NULL 
		DROP TABLE #EVENTOS
	
	SELECT EventoId 
	INTO #EVENTOS 
	FROM PresencaDatas 
	WHERE 
		Id = @DataId
		AND [Status] = 1 
	
	DELETE PresencaInscricaoDatas WHERE DataId = @DataId

	DELETE Evento
	WHERE Id IN (SELECT EventoId FROM #EVENTOS)

    DELETE PresencaDatas WHERE Id = @DataId
END