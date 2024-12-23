CREATE PROCEDURE [dbo].[ListarCongregacaoObservacao]
	@CongregacaoId INT
AS
BEGIN
	SELECT 
		CO.CongregacaoId,
		CO.UsuarioId AS UsuarioId,
		CO.Observacao,
		CO.DataCadastro,
		M.Nome AS UsuarioNome
	FROM CongregacaoObservacao CO
	INNER JOIN Congregacao C ON CO.CongregacaoId = C.Id
	INNER JOIN Usuario M ON M.Id = CO.UsuarioId
	WHERE
		CO.CongregacaoId = @CongregacaoId
	ORDER BY 
		CO.DataCadastro DESC
END
