CREATE PROCEDURE [dbo].[ListarBatismoCelebrante]
AS
BEGIN
	SELECT 
		*
	FROM
		Batismo B
	INNER JOIN BatismoCelebrante C ON B.Id = C.BatismoId
END
	
