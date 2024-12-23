CREATE PROCEDURE ListarPresencaPaginada
    @PAGESIZE INT,
	@ROWSTART INT,
	@SORTING VARCHAR(30),
	@USUARIOID INT,
    @CAMPO VARCHAR(255),
    @VALOR VARCHAR(255),
    @NAOMEMBRO BIT
AS
BEGIN 
    SET NOCOUNT ON
    DECLARE @SQLQUERY NVARCHAR(4000),
        @ORDERBY NVARCHAR(400),
        @PARMDEFINITION NVARCHAR(500),
        @SQLFILTRO NVARCHAR(4000)

    IF (CHARINDEX('Congregacao', @SORTING) > 0)
        SELECT 	@ORDERBY = 'C.Nome'
    ELSE IF (CHARINDEX('TipoEvento', @SORTING) > 0)
        SELECT 	@ORDERBY = 'T.Descricao'
    ELSE
        SELECT 	@ORDERBY = 'P.' + ISNULL(@SORTING, 'ID')

    SET @SQLFILTRO = ''
    IF (ISNULL(@CAMPO, '') <> '' AND ISNULL(@VALOR, '') <> '')
    BEGIN
        IF (@CAMPO IN ('CongregacaoId', 'TipoEventoId'))
            SET @SQLFILTRO = 'AND P.' + @CAMPO + ' = @VALOR'
        ELSE IF (@CAMPO = 'Status' AND @VALOR > 0)
            SET @SQLFILTRO = 'AND P.' + @CAMPO + ' = @VALOR'
        ELSE 
            SET @SQLFILTRO = ' AND P.' + @CAMPO + ' LIKE ''%' + @VALOR + '%'' COLLATE Latin1_General_CI_AI'
    END

    IF @NAOMEMBRO = 1
    BEGIN
        SET @SQLFILTRO = @SQLFILTRO + ' AND P.NaoMembros = 1'
    END

    SET @SQLQUERY = 
        'SELECT TOP(@PAGESIZE) * FROM (' +
        'SELECT' +
        '    P.Id,' +
        '    P.Descricao,' +
        '    P.TipoEventoId,' +
        '    T.Descricao AS TipoEvento,' +
        '    C.Nome AS Congregacao,' +
        '    P.CongregacaoId,' +
        '    P.DataMaxima,' +
        '    (SELECT MIN(D.DataHoraInicio) FROM PresencaDatas D WHERE D.PresencaId = P.Id) AS DataHoraInicio,' +
        '    P.[Status],' +
        '    ROW_NUMBER() OVER (ORDER BY ' + @ORDERBY + ') AS NUM ' +
        'FROM Presenca P ' +
        '   INNER JOIN TipoEvento T ON T.Id = P.TipoEventoId ' +
        '   INNER JOIN Congregacao C ON C.Id = P.CongregacaoId ' +
        'WHERE (P.ExclusivoCongregacao = 0 OR P.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@USUARIOID, default))) ' +
        @SQLFILTRO +
        ' ) AS A ' + 
        '  WHERE' +
        '    NUM > @ROWSTART'

    SET @SQLQUERY = @SQLQUERY + ' ORDER BY A.' + @SORTING

    /* EXECUCAÇÃO DA QUERY */
    SET @PARMDEFINITION = '@PAGESIZE INT, @ROWSTART INT, @VALOR VARCHAR(255), @USUARIOID INT'
    EXECUTE SP_EXECUTESQL @SQLQUERY, @PARMDEFINITION, @PAGESIZE, @ROWSTART, @VALOR, @USUARIOID


    DECLARE @SQLCOUNT NVARCHAR(4000)
    SET @SQLCOUNT = 'SELECT COUNT(*) AS ROWSCOUNT FROM Presenca P WITH(NOLOCK) '
    SET @SQLCOUNT = @SQLCOUNT + ' WHERE P.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@USUARIOID, default)) ' + @SQLFILTRO
    EXECUTE SP_EXECUTESQL @SQLCOUNT, @PARMDEFINITION,@PAGESIZE, @ROWSTART, @VALOR, @USUARIOID
END
