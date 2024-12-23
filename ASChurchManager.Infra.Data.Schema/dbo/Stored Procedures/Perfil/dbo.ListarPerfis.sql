
CREATE PROCEDURE dbo.ListarPerfis
AS
BEGIN

	SELECT 
		Id
		, Nome
		, TipoPerfil
		, Status
		, DataCriacao
		, DataAlteracao
	FROM dbo.Perfil

END