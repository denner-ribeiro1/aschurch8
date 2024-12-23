CREATE PROCEDURE [dbo].[ConsultarPresencaDatas]
	@PresencaId int
AS
BEGIN
	SELECT 
		* 
	FROM 
		PresencaDatas
	WHERE 
		PresencaId = @PresencaId
END
