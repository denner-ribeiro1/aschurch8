CREATE PROCEDURE [dbo].[DeletarCongregacaoGrupo]
	@CongregacaoId INT
AS
BEGIN
	DELETE dbo.CongregacaoGrupo
	WHERE CongregacaoId = @CongregacaoId
END