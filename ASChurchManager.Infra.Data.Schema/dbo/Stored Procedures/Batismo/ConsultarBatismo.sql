CREATE PROCEDURE dbo.ConsultarBatismo
	@Id INT
AS
BEGIN

	SELECT 
		Id 
		, DataMaximaCadastro
		, DataBatismo
		, DataCriacao
		, DataAlteracao
		, IdadeMinima
		, Status
	FROM dbo.Batismo
	WHERE
		Id = @Id

END
