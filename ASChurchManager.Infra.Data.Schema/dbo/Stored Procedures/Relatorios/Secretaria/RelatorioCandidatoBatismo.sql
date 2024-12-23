CREATE PROCEDURE [dbo].[RelatorioCandidatoBatismo]
	@BatismoId INT,
	@Congregacao BIGINT,
	@Situacao TINYINT,
	@UsuarioID BIGINT
AS
BEGIN
	SELECT 
		B.DataBatismo AS DataPrevistaBatismo,
		M.DataNascimento,
		M.Nome,
		C.Nome AS CongregacaoNome,
		CASE WHEN B.Status = '1' THEN 'Candidato ao Batismo'
		ELSE
			CASE WHEN BC.Situacao = 1 THEN 'Presente' ELSE 'Ausente' END
		END AS Situacao
	FROM BatismoCandidato BC
		INNER JOIN Batismo B ON B.Id = BC.BatismoId
		INNER JOIN Membro M ON M.Id = BC.MembroId
		INNER JOIN Congregacao C ON C.Id = M.CongregacaoId
	WHERE 
		(ISNULL(@BatismoId, 0) = 0 OR BC.BatismoId = @BatismoId) 
		AND (ISNULL(@Situacao, 0) = 0 OR BC.Situacao = @Situacao) 
		AND (ISNULL(@Congregacao, 0) = 0 OR M.CongregacaoId = @Congregacao) 
		AND C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
	ORDER BY
		C.Nome, M.Nome, B.DataBatismo
END