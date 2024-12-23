CREATE PROCEDURE DeletarConviteEvento
	@EventoId INT
AS
BEGIN
	DELETE 
		ConviteEvento
    WHERE
		EventoId = @EventoId
END