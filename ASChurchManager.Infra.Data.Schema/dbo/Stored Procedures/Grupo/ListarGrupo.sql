CREATE PROCEDURE dbo.ListarGrupo
AS
BEGIN
	
	SET NOCOUNT ON

	SELECT 
		Id 
		, Descricao
		, DataCriacao
		, DataAlteracao
	FROM dbo.Grupo

END
