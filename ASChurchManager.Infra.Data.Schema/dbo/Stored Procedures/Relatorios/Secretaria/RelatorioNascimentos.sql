CREATE PROCEDURE RelatorioNascimentos
	@DataInicio SMALLDATETIME, 
	@DataFinal SMALLDATETIME,
	@Congregacao INT,
	@UsuarioID INT
AS
BEGIN
	SELECT
		 N.Crianca,
		 CONVERT(VARCHAR, C.Id) + ' - ' + C.Nome AS CongregacaoNome,
		 N.Pastor,
		 N.DataApresentacao,
		 N.NomeMae,
		 N.NomePai,
		 N.DataNascimento,
		 CASE WHEN N.Sexo = 1 THEN 'M' ELSE 'F' END as Sexo
	FROM 
		Nascimento N		
		INNER JOIN dbo.Congregacao C ON N.CongregacaoId = C.Id
	WHERE 
		CONVERT(DATE, DataApresentacao) BETWEEN @DataInicio AND @DataFinal 
		AND (ISNULL(@Congregacao,0) = 0 OR N.CongregacaoID = @Congregacao )
		AND C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
	ORDER BY
		N.DataApresentacao, C.Id
END