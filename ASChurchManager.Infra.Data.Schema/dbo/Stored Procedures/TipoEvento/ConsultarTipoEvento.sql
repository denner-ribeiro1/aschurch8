CREATE PROCEDURE dbo.ConsultarTipoEvento
	@Id INT
AS
BEGIN
	SELECT 
		Id, 
		Descricao,
		DataCriacao,
		DataAlteracao
	FROM 
		dbo.TipoEvento
	WHERE
		Id = @Id
END