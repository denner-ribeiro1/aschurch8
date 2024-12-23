CREATE PROCEDURE dbo.ListarTemplates
AS
BEGIN

	SET NOCOUNT ON

	SELECT
		Id
		, Nome
		, Tipo
		, Status
		, DataCriacao
		, DataAlteracao
		, DataAlteracao
	FROM dbo.Template

END