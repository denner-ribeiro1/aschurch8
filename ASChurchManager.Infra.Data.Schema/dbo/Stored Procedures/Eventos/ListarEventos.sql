CREATE PROCEDURE ListarEventos
	@MESREF INT,
	@ANOREF INT, 
	@TIPOPESQUISA INT, 
	@CODIGOCONGR INT 
AS
BEGIN
	DECLARE @DataIni SMALLDATETIME, @DataFim SMALLDATETIME
	SET @DataIni = CONVERT(SMALLDATETIME, CONVERT(VARCHAR, @ANOREF) +  RIGHT('0' + CONVERT(VARCHAR, @MESREF), 2) + '01')
	SET @DataFim = DATEADD(DAY,-1, DATEADD(MONTH,1,@DataIni))

	IF OBJECT_ID('TEMPDB..#EVENTOS') IS NOT NULL 
		DROP TABLE #EVENTOS
	CREATE TABLE #EVENTOS (
		TipoEvento TINYINT,
		Id BIGINT,
		IdEventoOriginal BIGINT,
		CongregacaoId INT,
		CongregacaoNome VARCHAR(50),
		TipoEventoId INT,
		Descricao VARCHAR (500),
		DataHoraInicio DATETIMEOFFSET (7),
		DataHoraFim DATETIMEOFFSET (7),
		Observacoes VARCHAR(1000),
		Frequencia TINYINT NULL, 
		Quantidade INT NULL, 
		AlertarEventoMesmoDia BIT NULL
	) 
	
	SET NOCOUNT ON
	INSERT INTO #EVENTOS(
		TipoEvento,
		Id,
		IdEventoOriginal,
		CongregacaoId,
		CongregacaoNome,
		TipoEventoId,
		Descricao,
		DataHoraInicio,
		DataHoraFim,
		Observacoes,
		Frequencia,
		Quantidade,
		AlertarEventoMesmoDia
	)
	SELECT 
		1 AS TipoEvento
		, E.Id
		, E.IdEventoOriginal
		, E.CongregacaoId
		, C.Nome AS CongregacaoNome
		, E.TipoEventoId
		, E.Descricao
		, E.DataHoraInicio
		, E.DataHoraFim
		, E.Observacoes
		, E.Frequencia
		, E.Quantidade
		, E.AlertarEventoMesmoDia
	FROM Evento E
	INNER JOIN Congregacao C ON C.Id = E.CongregacaoId
	WHERE
		(@TIPOPESQUISA = 0
		 OR (@TIPOPESQUISA = 1 AND E.CongregacaoId = 1)
		 OR (@TIPOPESQUISA = 2 AND E.CongregacaoId = @CODIGOCONGR)
		 OR (@TIPOPESQUISA = 3 AND E.CongregacaoId <> @CODIGOCONGR AND E.CongregacaoId <> 1))
		AND E.DataHoraInicio BETWEEN @DataIni AND @DataFim
	UNION
	--CASAMENTOS A REALIZAR
	SELECT 
		2 AS TipoEvento
		, E.Id
		, NULL
		, E.CongregacaoId
		, C.Nome AS CongregacaoNome
		, NULL AS TipoEventoId
		, 'Casamento de ' + CASE WHEN ISNULL(E.NoivoId, '') = '' THEN E.NoivoNome ELSE Noivo.Nome END + ' e ' + CASE WHEN ISNULL(E.NoivaId, '') = '' THEN E.NoivaNome ELSE Noiva.Nome END AS Descricao
		, E.DataHoraInicio
		, E.DataHoraFinal
		, NULL AS Observacoes
		, NULL AS Frequencia
		, NULL AS Quantidade
		, NULL AS AlertarEventoMesmoDia
	FROM Casamento E
	Left Join Membro Noivo ON Noivo.Id = E.NoivoId
	Left Join Membro Noiva ON Noiva.Id = E.NoivaId
	Inner Join Congregacao C On C.Id = E.CongregacaoId
	Where
		(@TIPOPESQUISA = 0
		 OR (@TIPOPESQUISA = 1 AND E.CongregacaoId = 1)
		 OR (@TIPOPESQUISA = 2 AND E.CongregacaoId = @CODIGOCONGR)
		 OR (@TIPOPESQUISA = 3 AND E.CongregacaoId <> @CODIGOCONGR AND E.CongregacaoId <> 1))
		AND E.DataHoraInicio BETWEEN @DataIni AND @DataFim
	--BATISMO
	UNION
	SELECT 
		3 AS TipoEvento
		, B.Id
		, NULL
		, 1
		, 'IEAD-Mauá - Sede' AS CongregacaoNome
		, '' AS TipoEventoId
		, 'Batismo' AS Descricao
		, B.DataBatismo
		, NULL
		, '' AS Observacoes
		, NULL AS Frequencia
		, NULL AS Quantidade
		, NULL AS AlertarEventoMesmoDia
	FROM Batismo B
	WHERE
		B.DataBatismo BETWEEN @DataIni AND @DataFim
		AND B.Status = 1
	--CURSO/EVENTOS
	UNION
	SELECT
		4 AS TipoEvento
		, PD.Id
		, NULL
		, P.CongregacaoId
		, C.Nome AS CongregacaoNome
		, P.TipoEventoId AS TipoEventoId
		, P.Descricao AS Descricao
		, PD.DataHoraInicio
		, PD.DataHoraFim
		, 'A data máxima para inscrição no Curso/Evento é ' + CONVERT(varchar, P.DataMaxima, 103) AS Observacoes
		, NULL AS Frequencia
		, NULL AS Quantidade
		, NULL AS AlertarEventoMesmoDia
	FROM Presenca P
	INNER JOIN PresencaDatas PD ON P.Id = PD.PresencaId
	INNER JOIN Congregacao C On C.Id = P.CongregacaoId
	INNER JOIN TipoEvento T ON T.Id = P.TipoEventoId 
	WHERE 
		(@TIPOPESQUISA = 0
		 OR (@TIPOPESQUISA = 1 AND P.CongregacaoId = 1)
		 OR (@TIPOPESQUISA = 2 AND P.CongregacaoId = @CODIGOCONGR)
		 OR (@TIPOPESQUISA = 3 AND P.CongregacaoId <> @CODIGOCONGR AND P.CongregacaoId <> 1))
		AND PD.DataHoraInicio BETWEEN @DataIni AND @DataFim
		AND P.GerarEventos = 1
		AND ISNULL(PD.EventoId, 0) = 0


	SELECT * FROM #EVENTOS ORDER BY DataHoraInicio

	SELECT * FROM Feriados WHERE DataFeriado BETWEEN @DataIni AND @DataFim
END