CREATE PROCEDURE [dbo].[DeletarCongregacaoObreiro]
	@CongregacaoId INT
AS
BEGIN
	DELETE dbo.CongregacaoObreiro
	WHERE CongregacaoId = @CongregacaoId
END