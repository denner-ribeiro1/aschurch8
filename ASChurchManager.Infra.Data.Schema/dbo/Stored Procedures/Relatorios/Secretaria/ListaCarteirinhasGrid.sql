CREATE PROCEDURE ListaCarteirinhasGrid
	@TipoConsulta BIT,
    @Congregacao VARCHAR(MAX),
    @Cargos VARCHAR(MAX),
    @BatismoId INT,
    @ImprimirObrVinc BIT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE 
		@SQLQUERY NVARCHAR(MAX),
        @PARMDEFINITION NVARCHAR(500)

	SET @SQLQUERY =
    'SELECT DISTINCT M.Id, M.Nome, M.Cpf, C.Nome AS Congregacao, M.DataValidadeCarteirinha, CG.Descricao AS Cargo '+  
    'FROM Membro M '+  
    'INNER JOIN Congregacao C ON M.CongregacaoId = C.Id '+  
    'LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id) '+  
    'LEFT JOIN Cargo CG ON CG.Id = CM.CargoId '

    IF (@ImprimirObrVinc = 1)
    BEGIN
        SET @SQLQUERY = @SQLQUERY + ' INNER JOIN CongregacaoObreiro CO ON M.Id = CO.MembroId '
    END

    SET @SQLQUERY = @SQLQUERY + 
    'WHERE '+  
    '    M.[Status] = 1 ' +
    '    AND M.TipoMembro = 3 '


    IF @TipoConsulta = 0
    BEGIN
        IF (@ImprimirObrVinc = 1)
        BEGIN
            SET @SQLQUERY = @SQLQUERY + ' AND CO.CongregacaoId IN (' + @Congregacao + ') '
        END
        ELSE
        BEGIN
            SET @SQLQUERY = @SQLQUERY + ' AND M.CongregacaoId IN (' + @Congregacao + ') '
        END

        IF ISNULL(@Cargos, '') <> ''
        BEGIN
            IF (CHARINDEX('0', @Cargos) > 0)
                SET @SQLQUERY = @SQLQUERY + ' AND ISNULL(CM.CargoId, 0) IN (' + @Cargos + ')'
            ELSE
                SET @SQLQUERY = @SQLQUERY + ' AND CM.CargoId IN (' + @Cargos + ')'
        END
    END
    ELSE
    BEGIN
        SET @SQLQUERY = @SQLQUERY + ' AND M.BatismoId = @BatismoId'

        IF ISNULL(@Congregacao, '') <> ''
        BEGIN
            SET @SQLQUERY = @SQLQUERY + ' AND M.CongregacaoId IN (' + @Congregacao + ') '
        END
    END

    SET @PARMDEFINITION = '@BatismoId INT'
	EXECUTE SP_EXECUTESQL @SQLQUERY, @PARMDEFINITION, @BatismoId
END	