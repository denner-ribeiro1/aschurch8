CREATE PROCEDURE ListarCongregacaoGrupo
	@CongregacaoId INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT 
		GC.GrupoId 
		, G.Descricao AS Grupo
		, GC.NomeGrupo
		, GC.ResponsavelId
		, M.Nome AS ResponsavelNome
		, GC.DataCriacao
		, GC.CongregacaoId
	FROM CongregacaoGrupo GC
	INNER JOIN dbo.Grupo G ON GC.GrupoId = G.Id
	INNER JOIN dbo.Membro M ON M.Id = GC.ResponsavelId
	WHERE
		GC.CongregacaoId = @CongregacaoId
	ORDER BY G.Id DESC
END