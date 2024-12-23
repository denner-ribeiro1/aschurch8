CREATE PROCEDURE dbo.ListarCargos
AS
BEGIN
	
	SET NOCOUNT ON

	SELECT 
		Id 
		, Obreiro
		, Lider
		, TipoCarteirinha
		, Descricao
		, DataCriacao
		, DataAlteracao
		, Confradesp
		, CGADB
	FROM dbo.Cargo

END