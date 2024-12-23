
CREATE PROCEDURE dbo.ConsultarCargo
	@Id INT
AS
BEGIN

	SELECT 
		Id 
		, Descricao
		, Obreiro
		, Lider
		, TipoCarteirinha
		, DataCriacao
		, DataAlteracao
		, Confradesp
		, CGADB
	FROM dbo.Cargo
	WHERE
		Id = @Id

END