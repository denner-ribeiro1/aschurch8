
CREATE PROCEDURE dbo.ConsultarGrupo
	@Id INT
AS
BEGIN

	SELECT 
		Id 
		, Descricao
		, DataCriacao
		, DataAlteracao
	FROM dbo.Grupo
	WHERE
		Id = @Id

END