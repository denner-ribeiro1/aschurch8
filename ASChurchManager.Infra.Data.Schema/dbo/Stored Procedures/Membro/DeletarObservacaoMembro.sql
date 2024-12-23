CREATE PROCEDURE [dbo].[DeletarObservacaoMembro]
	@MembroId int
AS
BEGIN
	DELETE
		ObservacaoMembro
	WHERE
		MembroId = @MembroId
END