CREATE PROCEDURE RelatorioAniversariantes
	@DataInicio DATETIMEOFFSET, 
	@DataFinal DATETIMEOFFSET,
	@Congregacao INT,
	@TipoMembro INT,
	@UsuarioID INT
AS
BEGIN
	IF OBJECT_ID('TEMPDB..#ANIV') IS NOT NULL DROP TABLE #ANIV
	SELECT
		Id
		, CONVERT(SMALLDATETIME, 
			CONVERT(VARCHAR(4), YEAR(@DataInicio)) + 
			RIGHT('0' + CONVERT(VARCHAR(2),MONTH(M.DataNascimento)), 2) + 
			RIGHT('0' + CONVERT(VARCHAR(2),CASE WHEN dbo.VerificaAnoBissexto(YEAR(@DataInicio)) = 0 AND DAY(M.DataNascimento) > 28 THEN 28 ELSE DAY(M.DataNascimento) END), 2)
		) AS ANIVERSARIO
	INTO #ANIV
	FROM
		Membro M
	WHERE 
		(MONTH(M.DataNascimento) >= MONTH(@DataInicio) 
		AND MONTH(M.DataNascimento) <= MONTH(@DataFinal)) 
		AND	(@TipoMembro = 0 OR M.TipoMembro = @TipoMembro) 
		AND	M.Status = 1 
		AND (ISNULL(@Congregacao, 0) = 0 OR M.CongregacaoId = @Congregacao) 
		AND M.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
	ORDER BY M.DataNascimento
	
	SELECT 
		M.Nome
		, C.Nome AS Congregacao
		, M.DataNascimento
	FROM 
		Membro M
		INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
		INNER JOIN #ANIV A ON A.Id = M.Id
	WHERE 
		A.ANIVERSARIO BETWEEN @DataInicio AND @DataFinal
	ORDER BY 
		MONTH(M.DataNascimento), DAY(M.DataNascimento)
END
