CREATE PROCEDURE [dbo].[RelBatismoCandidatos]
	@BatismoId INT,
	@Situacao TINYINT,
	@Congregacao BIGINT,
	@UsuarioID BIGINT
AS
BEGIN
	SELECT
		M.Id,
		M.Nome,
		M.DataNascimento,
		C.Id AS CongregacaoId,
		C.Nome AS CongregacaoNome,
		CASE WHEN B.Status = '1' THEN 'Candidato ao Batismo'
		ELSE
			CASE WHEN BC.Situacao = 1 THEN 'Presente' ELSE 'Ausente' END
		END AS Situacao,
		M.TamanhoCapa
	FROM BatismoCandidato BC
		INNER JOIN Batismo B ON B.Id = BC.BatismoId
		INNER JOIN Membro M ON M.Id = BC.MembroId AND B.Id = M.BatismoId
		INNER JOIN Congregacao C ON C.Id = M.CongregacaoId
	WHERE 
		(ISNULL(@BatismoId, 0) = 0 OR BC.BatismoId = @BatismoId)
		AND (ISNULL(@Situacao, 0) = 0 OR BC.Situacao = @Situacao) 
		AND (ISNULL(@Congregacao, 0) = 0 OR M.CongregacaoId = @Congregacao) 
		AND C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
	ORDER BY
		C.Nome, M.Nome, B.DataBatismo
END
