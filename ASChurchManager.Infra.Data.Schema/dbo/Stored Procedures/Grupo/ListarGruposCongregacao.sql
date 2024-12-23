CREATE PROCEDURE [dbo].[ListarGruposCongregacao]
	@CongregacaoId int
AS
BEGIN
	SELECT
		R.Id AS Id,
		C.Id AS CongregacaoId,
		C.Nome AS CongregacaoNome,
		C.PastorResponsavelId AS CongregacaoResponsavelId,
		P.Nome AS CongregacaoResponsavelNome,
		R.GrupoId AS GrupoId,
		G.Descricao AS GrupoDescricao,
		R.Nome AS Nome,
		R.ResponsavelId AS ConvidadoId,
		M.Nome AS ConvidadoNome
	FROM
		GrupoCongregacao R
	INNER JOIN Grupo G ON G.Id = R.GrupoId
	INNER JOIN Congregacao C ON C.Id = R.CongregacaoId
	INNER JOIN Membro M ON M.Id = R.ResponsavelId
	LEFT JOIN Membro P ON P.Id = C.PastorResponsavelId
	WHERE
		R.CongregacaoId = @CongregacaoId
END
