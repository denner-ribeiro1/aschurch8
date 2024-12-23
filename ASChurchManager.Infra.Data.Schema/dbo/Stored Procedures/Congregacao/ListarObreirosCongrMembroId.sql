CREATE PROCEDURE ListarObreirosCongrMembroId
	@MembroId INT
AS
BEGIN
	SELECT 
		CongregacaoId, 
		MembroId AS ObreiroId, 
		C.Nome AS CongregacaoNome,
		NULL AS ObreiroNome, 
		NULL AS ObreiroCargo
	FROM 
		CongregacaoObreiro M
	INNER JOIN Congregacao C ON M.CongregacaoId = C.Id
	WHERE M.MembroId = @MembroId
	AND Dirigente = 0
END