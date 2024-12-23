
CREATE PROCEDURE dbo.ListarTipoEvento
AS
BEGIN
	SELECT 
		Id, 
		Descricao,
		DataCriacao,
		DataAlteracao
	FROM 
		dbo.TipoEvento
END