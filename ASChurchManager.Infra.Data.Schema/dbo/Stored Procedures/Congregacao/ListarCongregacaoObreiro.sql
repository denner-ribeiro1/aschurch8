CREATE PROCEDURE ListarCongregacaoObreiro
	@CongregacaoId INT
AS
BEGIN
	SELECT 
		CO.CongregacaoId,
		CO.MembroId AS ObreiroId,
		M.Nome AS ObreiroNome,
		CA.Descricao AS ObreiroCargo
	FROM CongregacaoObreiro CO
	INNER JOIN Congregacao C ON CO.CongregacaoId = C.Id
	INNER JOIN Membro M ON M.Id = CO.MembroId
	INNER JOIN CargoMembro CM ON CM.MembroId = CO.MembroId 
	INNER JOIN Cargo CA ON CM.CargoId = CA.Id
	WHERE
		CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = CM.MembroId)
		AND CO.CongregacaoId = @CongregacaoId
		AND CO.Dirigente = 0
	GROUP BY 
		CA.Descricao, CO.CongregacaoId, CO.MembroId, M.Nome 
	ORDER BY 
		CO.CongregacaoId
END
GO

