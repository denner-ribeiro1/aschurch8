CREATE PROCEDURE [dbo].[RelPresencaFrequenciaLista]
@Congregacao INT,
	@DataInicial SMALLDATETIME,
	@DataFinal SMALLDATETIME,
	@Cargos VARCHAR(200),
    @TipoEvento VARCHAR(200)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX),
			@FILTRO NVARCHAR(3000),
			@PARMDEFINITION NVARCHAR(500)
    
    SET @SQL = 
		'IF OBJECT_ID(''TEMPDB..#DATAS'') IS NOT NULL DROP TABLE #DATAS ' + CHAR(13) +
		'SELECT PD.* INTO #DATAS FROM Presenca P INNER JOIN PresencaDatas PD ON P.Id = PD.PresencaId ' + CHAR(13) +
		'WHERE CAST(PD.DataHoraInicio AS DATE) BETWEEN @DataInicial AND @DataFinal ' + CHAR(13)

    IF (ISNULL(@TipoEvento, '') <> '' )
        SET @SQL = @SQL + 'AND TipoEventoId IN (' + REPLACE(@TipoEvento, '_', ',') + ') ' + CHAR(13)

    SET @SQL = @SQL + 'ORDER BY PD.DataHoraInicio ASC ' + CHAR(13)


    SET @SQL = @SQL + 'IF OBJECT_ID(''TEMPDB..#INSCRICOES'') IS NOT NULL DROP TABLE #INSCRICOES ' + CHAR(13) +
                      'SELECT DISTINCT PI.* INTO #INSCRICOES FROM PresencaInscricao PI INNER JOIN #DATAS D ON PI.PresencaId = D.PresencaId ' + CHAR(13)

    SET @SQL = @SQL + 'IF OBJECT_ID(''TEMPDB..#INSCRDATAS'') IS NOT NULL DROP TABLE #INSCRDATAS' + CHAR(13) +
                      'SELECT PD.* INTO #INSCRDATAS FROM PresencaInscricaoDatas PD INNER JOIN #INSCRICOES I ON PD.InscricaoId = I.Id' + CHAR(13)

    SET @SQL = @SQL + 'SELECT DISTINCT ISNULL(M.Id,0) AS Id, ISNULL(M.Nome, I.Nome) AS Nome, ISNULL(C.Descricao, I.Cargo) AS Cargo,' + CHAR(13) +
                      ' ISNULL(M.CongregacaoId, 0) AS CongregacaoId, ISNULL(CG.Nome, I.Igreja) AS Congregacao, I.CPF' + CHAR(13) +
                      'FROM #INSCRICOES I ' + CHAR(13) +
                      'LEFT JOIN Membro M ON I.MembroId = M.Id ' + CHAR(13) +
                      'LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId ' + CHAR(13) +
                      'LEFT JOIN Cargo C ON C.Id = CM.CargoId ' + CHAR(13) +
                      'LEFT JOIN Congregacao CG ON CG.Id = M.CongregacaoId ' + CHAR(13) +
                      'WHERE  ' + CHAR(13) +
                      '    (M.[Status] IS NULL OR M.[Status] = 1) ' + CHAR(13) +
                      '    AND (CM.DataCargo IS NULL OR DataCargo = (SELECT MAX(DataCargo) FROM CargoMembro CM2 WHERE CM.MembroId = CM2.MembroId))' + CHAR(13)

    IF (ISNULL(@Cargos, '') <> '')
    BEGIN
        SET @SQL = @SQL + '    AND (ISNULL(CM.CargoId, 0) IN (' + REPLACE(@Cargos, '_', ',') + ')) ' + CHAR(13)
    END                  

    IF (ISNULL(@Congregacao, 0) > 0)
    BEGIN
        SET @SQL = @SQL + '    AND M.CongregacaoId = ' + CONVERT(VARCHAR, @Congregacao) + CHAR(13)
    END

    SET @SQL = @SQL + 'ORDER BY 2' + CHAR(13) +
                      'SELECT * FROM #INSCRICOES' + CHAR(13) +
                      'SELECT * FROM #DATAS ' + CHAR(13) + 
                      'SELECT * FROM #INSCRDATAS' + CHAR(13)
    
    SET @PARMDEFINITION = '@DataInicial SMALLDATETIME, @DataFinal SMALLDATETIME'
    EXECUTE SP_EXECUTESQL @SQL, @PARMDEFINITION, @DataInicial, @DataFinal
END