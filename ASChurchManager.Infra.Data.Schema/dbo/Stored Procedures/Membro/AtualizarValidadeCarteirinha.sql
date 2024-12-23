CREATE PROCEDURE [dbo].[AtualizarValidadeCarteirinha]
	@idMembro int
AS
BEGIN
	UPDATE
		Membro
	SET
		DataValidadeCarteirinha = CONVERT(datetimeoffset, CONVERT(date, DATEADD(YEAR, 2, GETDATE())))
	WHERE
		Id = @idMembro
END	
