CREATE PROCEDURE [dbo].[RelPresencaInscrito]
	@PresencaId INT,
	@CongregacaoId INT,
	@Tipo TINYINT,
	@UsuarioId INT
AS
BEGIN
	DECLARE @SQL NVARCHAR(3000),
			@FILTRO NVARCHAR(3000),
			@PARMDEFINITION NVARCHAR(500)

	SET @SQL =
		'SELECT DISTINCT' + 
		'	PI.Id AS Inscricao, ' +  
		'	ISNULL(ISNULL(CCO.Id, M.CongregacaoId), 0) AS CongregacaoId,  ' + 
		'	CASE WHEN PI.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE PI.Igreja END AS Igreja, ' + 
		'	P.Id, ' + 
		'	P.Descricao, ' + 
		'	ISNULL(PI.MembroId, 0) AS MembroId,  ' + 
		'	ISNULL(PI.Nome, M.Nome) AS Nome, ' + 
		'	ISNULL(PI.CPF, M.CPF) AS CPF, ' + 
		'	CASE WHEN PI.MembroId > 0 THEN ISNULL(CA.Descricao, ''Membro'') ELSE ISNULL(PI.Cargo, ''Visitante'') END AS Cargo, ' + 
		'	PI.Pago, ' + 
		'	P.Valor ' + 
		'FROM Presenca P ' + 
		'INNER JOIN PresencaInscricao PI ON PI.PresencaId = P.Id ' + 
		'LEFT JOIN Membro M ON M.Id = PI.MembroId ' + 
		'LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id ' + 
		'LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id) ' + 
		'LEFT JOIN Cargo CA ON CM.CargoId = CA.Id  ' + 
		'LEFT JOIN CongregacaoObreiro CO ON CO.MembroId = M.Id ' + 
		'LEFT JOIN Congregacao CCO ON CCO.Id = CO.CongregacaoId ' + 
		' WHERE 1 = 1'

	SET @FILTRO = ''
	IF @PresencaId > 0
		SET @FILTRO = @FILTRO + ' AND P.Id = @PresencaId'

	IF @CongregacaoId > 0
	BEGIN
		SET @FILTRO = @FILTRO + ' AND (ISNULL(ISNULL(CO.CongregacaoId, M.CongregacaoId), 0) = @CongregacaoId) ' 
	END

	-- MEMBRO
	IF @Tipo = 1
	BEGIN
		SET @FILTRO = @FILTRO + ' AND ISNULL(PI.MembroId, 0) > 0'
	END
	-- NAO MEMBRO
	ELSE IF @Tipo = 2
	BEGIN
		SET @FILTRO = @FILTRO + ' AND ISNULL(PI.MembroId, 0) = 0'
	END

	--- USUARIO NAO E DA SEDE
	IF @UsuarioId > 0 AND NOT EXISTS(SELECT TOP 1 1 FROM Usuario U
			      INNER JOIN Congregacao C ON U.CongregacaoId = C.Id
				  WHERE U.Id = @UsuarioId
					AND C.Sede = 1) 
	BEGIN
		 SET @FILTRO = @FILTRO + ' AND ISNULL(CO.CongregacaoId, M.CongregacaoId) IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioId, default))'
	END

	SET @SQL = @SQL + @FILTRO + ' ORDER BY  ' + 
								' 	ISNULL(PI.Nome, M.Nome), ' + 
								' 	PI.Id, ' +  	
								' 	ISNULL(ISNULL(CCO.Id, M.CongregacaoId), 0),  ' + 
								' 	CASE WHEN PI.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE PI.Igreja END,  ' + 
								' 	P.Id,  ' + 
								' 	P.Descricao, ' +  	
								' 	ISNULL(PI.MembroId, 0), ' +   	
								' 	ISNULL(PI.CPF, M.CPF),  ' + 
								' 	CASE WHEN PI.MembroId > 0 THEN ISNULL(CA.Descricao, ''Membro'') ELSE ISNULL(PI.Cargo, ''Visitante'') END,  ' + 
								' 	PI.Pago, ' + 
								' 	P.Valor  '

	SET @PARMDEFINITION = '@PresencaId INT,@CongregacaoId INT,@Tipo INT,@UsuarioId INT'
    EXECUTE SP_EXECUTESQL @SQL, @PARMDEFINITION, @PresencaId, @CongregacaoId, @Tipo, @UsuarioId
END
