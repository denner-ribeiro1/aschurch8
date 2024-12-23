CREATE PROCEDURE dbo.RelatorioEventos
	@CongregacaoId INT,
	@Mes TINYINT, 
	@Ano INT, 
	@TipoEvento INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	DECLARE @DT1 DATETIME, @DT2 DATETIME
	IF (ISNULL(@Ano, 0) = 0)
	BEGIN
		EXEC Util_RetornaPeriodoMes 1, 2000, @DT1 OUTPUT, @DT2 OUTPUT
		SET @DataIni = @DT1

		EXEC Util_RetornaPeriodoMes 12, 2050, @DT1 OUTPUT, @DT2 OUTPUT
		SET @DataFim = @DT2
	END
	ELSE IF (ISNULL(@Mes, 0) = 0)
	BEGIN
		EXEC Util_RetornaPeriodoMes 1, @Ano, @DT1 OUTPUT, @DT2 OUTPUT
		SET @DataIni = @DT1

		EXEC Util_RetornaPeriodoMes 12, @Ano, @DT1 OUTPUT, @DT2 OUTPUT
		SET @DataFim = @DT2
	END
	ELSE
	BEGIN
		EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT
	END

	IF OBJECT_ID('TEMPDB..#EVENTOS') IS NOT NULL 
		DROP TABLE #EVENTOS
	CREATE TABLE #EVENTOS (
		TipoEvento TINYINT,
		Id BIGINT,
		CongregacaoNome VARCHAR(50),
		Descricao VARCHAR (500),
		DataHoraInicio DATETIMEOFFSET (7),
		DataHoraFim DATETIMEOFFSET (7)
	) 
	
	SET NOCOUNT ON
	INSERT INTO #EVENTOS(
		TipoEvento,
		Id,
		CongregacaoNome,
		Descricao,
		DataHoraInicio,
		DataHoraFim
	)
	SELECT 
		1 AS TipoEvento
		, E.Id
		, C.Nome AS CongregacaoNome
		, E.Descricao
		, E.DataHoraInicio
		, E.DataHoraFim
	FROM Evento E
	INNER JOIN Congregacao C ON C.Id = E.CongregacaoId
	LEFT JOIN TipoEvento T ON T.Id = E.TipoEventoId
	WHERE
		CONVERT(SMALLDATETIME, E.DataHoraInicio ) BETWEEN @DataIni AND @DataFim
		AND (ISNULL(@CongregacaoId, 0) = 0 OR C.Id = @CongregacaoId)
	UNION
	--CASAMENTOS A REALIZAR
	SELECT 
		2 AS TipoEvento
		, E.Id
		, C.Nome AS CongregacaoNome
		, 'Casamento de ' + CASE WHEN ISNULL(E.NoivoId, '') = '' THEN E.NoivoNome ELSE Noivo.Nome END + ' e ' + CASE WHEN ISNULL(E.NoivaId, '') = '' THEN E.NoivaNome ELSE Noiva.Nome END AS Descricao
		, E.DataHoraInicio
		, E.DataHoraFinal
	FROM Casamento E
	Left Join Membro Noivo ON Noivo.Id = E.NoivoId
	Left Join Membro Noiva ON Noiva.Id = E.NoivaId
	Inner Join Congregacao C On C.Id = E.CongregacaoId
	Where
		CONVERT(SMALLDATETIME, E.DataHoraInicio) BETWEEN @DataIni AND @DataFim
		AND (ISNULL(@CongregacaoId, 0) = 0 OR C.Id = @CongregacaoId)
	--BATISMO
	UNION
	SELECT 
		3 AS TipoEvento
		, B.Id
		, 'IEAD-Mauá - Sede' AS CongregacaoNome
		, 'Batismo' AS Descricao
		, B.DataBatismo
		, NULL
	FROM Batismo B
	WHERE
		CONVERT(SMALLDATETIME, B.DataBatismo) BETWEEN @DataIni AND @DataFim
		AND (ISNULL(@CongregacaoId, 0) <= 1)

	SELECT * FROM #EVENTOS 
	WHERE 
		(ISNULL(@TipoEvento, 0) = 0 OR TipoEvento = @TipoEvento)
	ORDER BY DataHoraInicio

	SELECT * FROM Feriados WHERE DataFeriado BETWEEN @DataIni AND @DataFim
END