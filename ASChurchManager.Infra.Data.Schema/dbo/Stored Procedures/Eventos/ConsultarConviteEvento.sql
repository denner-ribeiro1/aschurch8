CREATE PROCEDURE [dbo].[ConsultarConviteEvento]
	@EventoId int
AS
BEGIN
	SELECT 
		* 
	FROM
		ConviteEvento
	WHERE
		EventoId = @EventoId
END