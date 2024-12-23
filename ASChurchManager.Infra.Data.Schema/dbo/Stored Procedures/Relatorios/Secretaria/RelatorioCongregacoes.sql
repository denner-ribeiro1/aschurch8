CREATE PROCEDURE RelatorioCongregacoes
	@Congregacao INT,
	@UsuarioID INT
AS
BEGIN
	SELECT 
		 M.Nome AS Dirigente
		,C.Nome AS Congregacao
		,C.Id
		,C.Logradouro
		,C.CNPJ
		,C.Numero
		,ISNULL(C.Complemento,' - ') AS Complemento
		,C.Cep
		,C.Bairro
		,C.Cidade
		,C.Estado
		,C.Pais
		,(SELECT COUNT(CO.CongregacaoId) FROM CongregacaoObreiro CO WHERE CO.CongregacaoId = C.Id) AS QtdObreiros
		,(SELECT COUNT(MA.Id) FROM Membro MA WHERE MA.CongregacaoId = C.Id AND MA.TipoMembro = 3 AND  MA.Status = 1) AS QtdMembrosAtivos
	FROM 
		Congregacao C		
		LEFT JOIN Membro M ON C.PastorResponsavelId = M.Id
	WHERE 
		(ISNULL(@Congregacao, 0) = 0 OR C.Id = @Congregacao) 
		AND	C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
		AND C.Situacao = 1
END