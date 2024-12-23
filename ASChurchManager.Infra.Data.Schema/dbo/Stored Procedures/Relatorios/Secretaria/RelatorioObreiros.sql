CREATE PROCEDURE RelatorioObreiros
	@Congregacao INT,
	@UsuarioID INT
AS
BEGIN
	SELECT 
		CA.Descricao,
		CONVERT(VARCHAR, M.Id) + ' - ' + M.Nome AS NomeMembro,
		M.DataNascimento,
		CONVERT(VARCHAR, C.Id) + ' - ' +  C.Nome AS CongregacaoNome,
		C.Id AS CongregacaoId,
		CASE WHEN P.Id IS NOT NULL THEN CONVERT(VARCHAR, P.Id) + ' - ' + P.Nome ELSE NULL END AS PastorResponsavel
	FROM CongregacaoObreiro CO
	INNER JOIN Congregacao C ON CO.CongregacaoId = C.Id
	INNER JOIN Membro M ON M.Id = CO.MembroId
	INNER JOIN CargoMembro CM ON CM.MembroId = CO.MembroId 
	INNER JOIN Cargo CA ON CM.CargoId = CA.Id
	LEFT JOIN Membro P ON P.Id = C.PastorResponsavelId
	WHERE
		CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = CM.MembroId)
		AND (ISNULL(@Congregacao, 0) = 0 OR CO.CongregacaoId = @Congregacao)
		AND	C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
		AND Dirigente = 0
	GROUP BY 
		CA.Descricao, C.Nome, C.Id, M.Id, M.Nome, M.DataNascimento, P.Id, P.Nome 
	ORDER BY 
		C.Id
END