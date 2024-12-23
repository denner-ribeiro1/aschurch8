CREATE PROCEDURE [dbo].[DeletarCongregacaoObservacao]
	@CongregacaoId INT
AS
BEGIN
	DELETE dbo.CongregacaoObservacao
	WHERE CongregacaoId = @CongregacaoId
END
