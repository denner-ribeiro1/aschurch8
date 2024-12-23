CREATE PROCEDURE ListarCursoArquivoMembro
	@MembroId int
AS
BEGIN
	SELECT A.Id
		  ,A.MembroId
		  ,A.TipoArquivo
		  ,A.Descricao
		  ,A.CursoId
		  ,CASE WHEN ISNULL(A.CursoId, 0) = 0 THEN A.Local ELSE 'Sede' END AS Local
		  ,CASE WHEN ISNULL(A.CursoId, 0) = 0 THEN A.NomeCurso ELSE C.Descricao END AS NomeCurso
		  ,CASE WHEN ISNULL(A.CursoId, 0) = 0 THEN A.DataInicioCurso ELSE C.DataInicio END AS DataInicioCurso
		  ,CASE WHEN ISNULL(A.CursoId, 0) = 0 THEN A.DataEncerramentoCurso ELSE C.DataEncerramento END AS DataEncerramentoCurso
		  ,CASE WHEN ISNULL(A.CursoId, 0) = 0 THEN A.CargaHoraria ELSE C.CargaHoraria END AS CargaHoraria
		  ,NomeArmazenado
		  ,NomeOriginal
		  ,Tamanho
		  ,A.DataCriacao
	FROM CursoArquivoMembro A
	LEFT JOIN Curso C ON C.Id = A.CursoId
	WHERE
		MembroId = @MembroId
END
