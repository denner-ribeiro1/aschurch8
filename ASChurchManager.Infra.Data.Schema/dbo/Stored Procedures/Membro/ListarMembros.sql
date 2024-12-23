CREATE PROCEDURE [dbo].[ListarMembros]
	@UsuarioID INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT
		M.Id 
		, M.CongregacaoId
		, CongregacaoNome = C.Nome
		, M.Nome
		, Situacao = (SELECT S.Situacao 
						FROM SituacaoMembro S 
						WHERE S.MembroId = M.Id 
						AND S.Id = (SELECT MAX(X.Id) FROM SituacaoMembro X 
									WHERE X.MembroId = M.Id)) 
		, M.Status
		, M.DataBatismoAguas
		, M.DataPrevistaBatismo
		, M.TipoMembro
		, M.DataNascimento
		, M.NomeMae
		, M.DataCriacao
		, M.DataAlteracao
	FROM dbo.Membro M
	LEFT JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
	WHERE 
		C.Id IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
END