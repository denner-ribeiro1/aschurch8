CREATE PROCEDURE ConsultarCurso
	@Id INT
AS
BEGIN
	SELECT 
		Id
		,Descricao
		,DataInicio
		,DataEncerramento
		,CargaHoraria
		,DataCriacao
		,DataAlteracao
	FROM 
		dbo.Curso
	WHERE
		Id = @Id
END