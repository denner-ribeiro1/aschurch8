CREATE PROCEDURE [dbo].[ListarPresencaInscricoesDatas]
	@PresencaId INT,
	@DataId INT,
	@PAGESIZE INT,
	@ROWSTART INT,
	@SORTING VARCHAR(30),
	@USUARIOID INT,
    @CAMPO VARCHAR(255),
    @VALOR VARCHAR(255)
AS
BEGIN
	IF OBJECT_ID('TEMPDB..#INSCR') IS NOT NULL DROP TABLE #INSCR
	SELECT
		I.InscricaoId AS Id,
		D.PresencaId, 
		I.DataId,
		I.Situacao,
		I.Tipo,
		I.Justificativa,
		D.Status AS StatusData
	INTO
		#INSCR
	FROM PresencaDatas D
	INNER JOIN PresencaInscricaoDatas I ON I.DataId = D.Id
	WHERE 
		D.PresencaId = @PresencaId
		AND I.DataId = @DataId
	
	INSERT INTO #INSCR
	SELECT
		P.Id,
		P.PresencaId,
		@DataId AS DataId,
		2 AS Situacao,
		0 AS Tipo,
		'' AS Justificativa,
		(SELECT D.Status FROM PresencaDatas D WHERE D.Id = @DataId) AS StatusData
	FROM
		PresencaInscricao P
	WHERE 
		P.PresencaId = @PresencaId
		AND NOT EXISTS(SELECT TOP 1 1 FROM #INSCR X WHERE X.Id = P.Id) 

	IF OBJECT_ID('TEMPDB..#INSCRDATAS') IS NOT NULL DROP TABLE #INSCRDATAS
	SELECT DISTINCT
		P.Id,
		P.PresencaId,
		PI.MembroId,
		ISNULL(PI.Nome, M.Nome) AS Nome,
		ISNULL(PI.CPF, M.CPF) AS CPF,
		ISNULL(ISNULL(CCO.Id, M.CongregacaoId), 0) AS CongregacaoId,
		CASE WHEN PI.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE PI.Igreja END AS Igreja,
		CASE WHEN PI.MembroId > 0 THEN ISNULL(CA.Descricao, 'Membro') ELSE ISNULL(PI.Cargo, 'Visitante') END AS Cargo,
		PI.Pago,
		PI.DataCriacao,
		PI.DataAlteracao,
		PI.Usuario,
		P.Situacao,
		P.Tipo,
		P.Justificativa,
		P.StatusData
	INTO
		#INSCRDATAS
	FROM  
		#INSCR P
	INNER JOIN PresencaInscricao PI ON P.Id = PI.Id
	LEFT JOIN Membro M ON M.Id = PI.MembroId
	LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id
	LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id)
	LEFT JOIN Cargo CA ON CM.CargoId = CA.Id 
	LEFT JOIN CongregacaoObreiro CO ON CO.MembroId = M.Id 
	LEFT JOIN Congregacao CCO ON CCO.Id = CO.CongregacaoId 


	SET NOCOUNT ON
    DECLARE @SQLQUERY NVARCHAR(4000),
        @ORDERBY NVARCHAR(400),
        @PARMDEFINITION NVARCHAR(500),
        @SQLFILTRO NVARCHAR(4000)

	SELECT @ORDERBY = 'P.' + ISNULL(@SORTING, 'Id')

    SET @SQLFILTRO = ''
    IF (ISNULL(@CAMPO, '') <> '' AND ISNULL(@VALOR, '') <> '')
    BEGIN
		SET @SQLFILTRO = ' WHERE P.' + @CAMPO + ' LIKE ''%' + @VALOR + '%'' COLLATE Latin1_General_CI_AI'
    END

	SET @SQLQUERY = 
        'SELECT TOP(@PAGESIZE) * FROM (' +
		'SELECT' +
		'    P.Id,' +
		'    P.PresencaId,' +
		'    P.MembroId,' +
		'    P.Nome,' +
		'    P.CPF,' +
		'    P.CongregacaoId,' +
		'    P.Igreja,' +
		'    P.Cargo,' +
		'    P.Pago,' +
		'    P.DataCriacao,' +
		'    P.DataAlteracao,' +
		'    P.Usuario,' +
		'    P.Situacao,' +
		'    P.Tipo,' +
		'    P.Justificativa,' +
		'    P.StatusData,' +
        '    ROW_NUMBER() OVER (ORDER BY ' + @ORDERBY + ') AS NUM ' +
        'FROM #INSCRDATAS P ' +
        @SQLFILTRO +
        ' ) AS P ' + 
        '  WHERE' +
        '    NUM > @ROWSTART'

    SET @SQLQUERY = @SQLQUERY + ' ORDER BY ' + @ORDERBY

    /* EXECUCAÇÃO DA QUERY */
    SET @PARMDEFINITION = '@PAGESIZE INT, @ROWSTART INT, @VALOR VARCHAR(255), @USUARIOID INT'
    EXECUTE SP_EXECUTESQL @SQLQUERY, @PARMDEFINITION, @PAGESIZE, @ROWSTART, @VALOR, @USUARIOID


    DECLARE @SQLCOUNT NVARCHAR(4000)
    SET @SQLCOUNT = 'SELECT COUNT(*) AS ROWSCOUNT FROM #INSCRDATAS P WITH(NOLOCK) ' + @SQLFILTRO
    EXECUTE SP_EXECUTESQL @SQLCOUNT, @PARMDEFINITION,@PAGESIZE, @ROWSTART, @VALOR, @USUARIOID
END
