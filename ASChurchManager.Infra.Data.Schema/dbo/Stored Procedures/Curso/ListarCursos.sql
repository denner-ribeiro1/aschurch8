CREATE PROCEDURE ListarCursos
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
		Curso
END