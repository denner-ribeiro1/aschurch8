CREATE PROCEDURE [dbo].[DeletarSituacaoMembro]
	@MembroId INT
AS
BEGIN
	DELETE 
		dbo.SituacaoMembro
	WHERE
		MembroId = @MembroId
END