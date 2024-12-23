CREATE PROCEDURE [dbo].[ConsultarPresencaEtiquetas]
	@PresencaId INT,
	@CongregacaoId INT = NULL,
	@Tipo INT = NULL,
	@MembroId INT  = NULL,
	@CPF VARCHAR(15) = NULL,
	@UsuarioId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(3000),
			@FILTRO NVARCHAR(3000),
			@PARMDEFINITION NVARCHAR(500)

	SET @SQL = 'SELECT ' + 
				'	P.Id, ' +
				'	P.MembroId, ' +
				'	ISNULL(P.Nome, M.Nome) AS Nome, ' +
				'	CASE WHEN P.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE P.Igreja END AS Igreja,' +
				'	ISNULL(P.Cargo, CA.Descricao) AS Cargo ' +
				'FROM' +
				'	PresencaInscricao P' +
				'	LEFT JOIN Membro M ON M.Id = P.MembroId' +
				'	LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id' +
				'	LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id) ' +
				'	LEFT JOIN Cargo CA ON CM.CargoId = CA.Id ' +
				'	LEFT JOIN CongregacaoObreiro CO ON CO.MembroId = M.Id  ' +
				'	LEFT JOIN Congregacao CCO ON CCO.Id = CO.CongregacaoId  ' +
				'WHERE' +
				'	P.PresencaId =  @PresencaId' + 
				'   AND (ISNULL(M.CongregacaoId, 0) = 0 OR M.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioId, default)) )'
	
	
	SET @FILTRO = ''
	IF @CongregacaoId > 0
		SET @FILTRO = @FILTRO + ' AND ISNULL(CO.CongregacaoId, M.CongregacaoId) = @CongregacaoId'
	
	-- MEMBRO
	IF @Tipo = 1
	BEGIN
		IF ISNULL(@MembroId, 0) > 0
			SET @FILTRO = @FILTRO + ' AND P.MembroId = @MembroId'
		ELSE
			SET @FILTRO = @FILTRO + ' AND ISNULL(P.MembroId, 0) > 0'
	END
	-- NAO MEMBRO
	ELSE IF @Tipo = 2
	BEGIN
		IF ISNULL(@CPF, '') <> '' 
			SET @FILTRO = @FILTRO + ' AND P.CPF = @CPF'
		ELSE
			SET @FILTRO = @FILTRO + ' AND ISNULL(P.MembroId, 0) = 0'
	END

	SET @SQL = @SQL + @FILTRO + ' ORDER BY 1'


	SET @PARMDEFINITION = '@PresencaId INT,@CongregacaoId INT,@Tipo INT,@MembroId INT,@CPF VARCHAR(15),@UsuarioId INT'
    EXECUTE SP_EXECUTESQL @SQL, @PARMDEFINITION, @PresencaId, @CongregacaoId, @Tipo, @MembroId, @CPF, @UsuarioId
END