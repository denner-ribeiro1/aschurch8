CREATE PROCEDURE [dbo].[ConsultarPresencaDatasEmAndamento]
AS
BEGIN
    DECLARE @DATAATUAL DATETIME

    SET @DATAATUAL = GETDATE()
    IF (CHARINDEX('AZURE', UPPER(@@VERSION)) > 0 )
    BEGIN
        SET @DATAATUAL = DATEADD(SECOND, -10800, GETDATE())
    END

    SELECT * FROM PresencaDatas WHERE [Status] = 2
    AND @DATAATUAL > DataHoraFim 
    UNION
    SELECT * FROM PresencaDatas WHERE [Status] = 2
    AND GETDATE() > DATEADD(DAY, 1, DataHoraRegistro) 
END