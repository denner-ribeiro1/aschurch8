CREATE PROCEDURE DeletarCursoArquivoMembro
	@Id int
AS
BEGIN
	DELETE
		CursoArquivoMembro
	WHERE
		Id = @Id
END