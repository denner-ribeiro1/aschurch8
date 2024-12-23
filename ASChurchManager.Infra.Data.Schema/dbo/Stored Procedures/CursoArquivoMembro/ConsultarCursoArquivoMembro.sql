CREATE PROCEDURE ConsultarCursoArquivoMembro
	@Id int
AS
BEGIN
	SELECT Id
		  ,MembroId
		  ,TipoArquivo
		  ,Descricao
		  ,CursoId
		  ,Local
		  ,NomeCurso
		  ,DataInicioCurso
		  ,DataEncerramentoCurso
		  ,CargaHoraria
		  ,NomeArmazenado
		  ,NomeOriginal
		  ,Tamanho
		  ,DataCriacao
	FROM CursoArquivoMembro
	WHERE
		Id = @Id
END