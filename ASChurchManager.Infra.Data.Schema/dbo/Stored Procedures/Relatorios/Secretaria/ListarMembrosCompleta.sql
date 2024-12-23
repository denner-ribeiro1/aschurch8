CREATE PROCEDURE ListarMembrosCompleta
	@CongregacaoID INT,
	@Status INT,
	@TipoMembro INT,
	@EstadoCivil INT,
	@ABEDABE BIT,
	@UsuarioID INT,
	@FiltrarConf INT,
	@AtivosConf BIT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @SQL NVARCHAR(2500), @ParamDef nvarchar(500)

		
	SET @SQL = N'SELECT' + CHAR(13) +
				'	M.Status, M.TipoMembro,' + CHAR(13) +
				'	CONVERT(VARCHAR, C.Id) + '' - '' + C.Nome AS CongregacaoNome,' + CHAR(13) +
				'	CONVERT(VARCHAR, M.Id) + '' - '' + M.Nome AS Membro,' + CHAR(13) +
				'	M.Id AS MembroId, M.Nome AS MembroNome,' + CHAR(13) +
				'	M.NomePai, M.NomeMae, M.DataNascimento, M.EstadoCivil,' + CHAR(13) +
				'	(CASE WHEN ISNULL(M.NaturalidadeCidade, '''') <> '''' OR  ISNULL(M.NaturalidadeEstado, '''') <> '''' OR ISNULL(M.Nacionalidade, '''') <> '''' THEN' + CHAR(13) +
				'		CASE WHEN ISNULL(M.NaturalidadeCidade, '''') <> '''' THEN M.NaturalidadeCidade ELSE '''' END + ' + CHAR(13) +
				'		CASE WHEN ISNULL(M.NaturalidadeEstado, '''') <> '''' THEN ''/'' + M.NaturalidadeEstado ELSE '''' END + ' + CHAR(13) +
				'		CASE WHEN ISNULL(M.Nacionalidade, '''') <> '''' THEN '' - '' + M.Nacionalidade ELSE '''' END' + CHAR(13) +
				'	ELSE '''' END) AS Natural,' + CHAR(13) +
				'	SUBSTRING((CASE WHEN ISNULL(M.TelefoneCelular, '''') <> '''' OR  ISNULL(M.TelefoneResidencial, '''') <> '''' OR ISNULL(M.TelefoneComercial, '''') <> '''' THEN' + CHAR(13) +
				'		CASE WHEN ISNULL(M.TelefoneCelular, '''') <> '''' THEN '' - Cel.: '' + M.TelefoneCelular ELSE '''' END +' + CHAR(13) +
				'		CASE WHEN ISNULL(M.TelefoneResidencial, '''') <> '''' THEN '' - Res.: '' + M.TelefoneResidencial ELSE '''' END +' + CHAR(13) +
				'		CASE WHEN ISNULL(M.TelefoneComercial, '''') <> '''' THEN '' - Com.: '' + M.TelefoneComercial ELSE '''' END ' + CHAR(13) +
				'	ELSE '''' END), 4, LEN(CASE WHEN ISNULL(M.TelefoneCelular, '''') <> '''' OR  ISNULL(M.TelefoneResidencial, '''') <> '''' OR ISNULL(M.TelefoneComercial, '''') <> '''' THEN' + CHAR(13) +
				'		CASE WHEN ISNULL(M.TelefoneCelular, '''') <> '''' THEN '' - Cel.: '' + M.TelefoneCelular ELSE '''' END +' + CHAR(13) +
				'		CASE WHEN ISNULL(M.TelefoneResidencial, '''') <> '''' THEN '' - Res.: '' + M.TelefoneResidencial ELSE '''' END +' + CHAR(13) +
				'		CASE WHEN ISNULL(M.TelefoneComercial, '''') <> '''' THEN '' - Com.: '' + M.TelefoneComercial ELSE '''' END ' + CHAR(13) +
				'	ELSE '''' END)) AS Telefones, ' + CHAR(13) +
				'	Situacao = (SELECT MAX(S.Situacao) FROM SituacaoMembro S WHERE S.MembroId = M.Id AND S.Data = (SELECT MAX(X.Data) FROM SituacaoMembro X WHERE X.MembroId = M.Id)),' + CHAR(13) +
				'	CASE WHEN ISNULL(M.ABEDABE, 0) = 1 THEN ''Sim'' ELSE ''Não'' END AS MembroAbedabe' + CHAR(13) +
				'FROM ' + CHAR(13) +
				'	Membro M' + CHAR(13) +
				'	INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id' + CHAR(13) +
				'WHERE C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))' + CHAR(13)

	IF(ISNULL(@CongregacaoID, 0) > 0)
		SET @SQL = @SQL + 'AND M.CongregacaoId = @CongregacaoID ' + CHAR(13)

	IF(ISNULL(@TipoMembro, 0) > 0)
		SET @SQL = @SQL + 'AND M.TipoMembro = @TipoMembro ' + CHAR(13)
		
	IF(ISNULL(@Status, 0) > 0)
		SET @SQL = @SQL + 'AND M.Status = @Status ' + CHAR(13)

	IF(ISNULL(@EstadoCivil, 0) > 0)
		SET @SQL = @SQL + 'AND M.EstadoCivil = @EstadoCivil ' + CHAR(13)

	IF(ISNULL(@ABEDABE, 0) = 1)
		SET @SQL = @SQL + 'AND ISNULL(M.ABEDABE,0) = 1'
	
	IF(ISNULL(@FiltrarConf,0) = 1)
	BEGIN
		IF (ISNULL(@AtivosConf,0) = 1)
			SET @SQL = @SQL + 'AND EXISTS(SELECT TOP 1 1 FROM HistoricoMembro H WHERE H.Id = M.Id) '
		ELSE
			SET @SQL = @SQL + 'AND NOT EXISTS(SELECT TOP 1 1 FROM HistoricoMembro H WHERE H.Id = M.Id) '
	END

	SET @ParamDef = N'@CongregacaoID INT, @Status INT, @TipoMembro INT, @EstadoCivil INT, @UsuarioID INT'

	EXECUTE sp_executesql @SQL, @ParamDef, @CongregacaoID, @Status, @TipoMembro, @EstadoCivil, @UsuarioID 

	----UTILIZADO PARA MONTAR A ESTRUTURA DO DATASET DO RELATORIO
	--SELECT TOP 10
	--	M.Status, M.TipoMembro,
	--	CONVERT(VARCHAR, C.Id) + ' - ' + C.Nome AS CongregacaoNome,
	--	CONVERT(VARCHAR, M.Id) + ' - ' + M.Nome AS Membro,
	--	M.NomePai, M.NomeMae, M.DataNascimento, M.EstadoCivil,
	--	(CASE WHEN ISNULL(M.NaturalidadeCidade, '') <> '' OR  ISNULL(M.NaturalidadeEstado, '') <> '' OR ISNULL(M.Nacionalidade, '') <> '' THEN
	--		CASE WHEN ISNULL(M.NaturalidadeCidade, '') <> '' THEN M.NaturalidadeCidade ELSE '' END + 
	--		CASE WHEN ISNULL(M.NaturalidadeEstado, '') <> '' THEN '/' + M.NaturalidadeEstado ELSE '' END + 
	--		CASE WHEN ISNULL(M.Nacionalidade, '') <> '' THEN ' - ' + M.Nacionalidade ELSE '' END
	--	ELSE '' END) AS Natural,
	--	SUBSTRING((CASE WHEN ISNULL(M.TelefoneCelular, '') <> '' OR  ISNULL(M.TelefoneResidencial, '') <> '' OR ISNULL(M.TelefoneComercial, '') <> '' THEN
	--		CASE WHEN ISNULL(M.TelefoneCelular, '') <> '' THEN ' - Cel.: ' + M.TelefoneCelular ELSE '' END +
	--		CASE WHEN ISNULL(M.TelefoneResidencial, '') <> '' THEN ' - Res.: ' + M.TelefoneResidencial ELSE '' END +
	--		CASE WHEN ISNULL(M.TelefoneComercial, '') <> '' THEN ' - Com.: ' + M.TelefoneComercial ELSE '' END 
	--	ELSE '' END), 4, LEN(CASE WHEN ISNULL(M.TelefoneCelular, '') <> '' OR  ISNULL(M.TelefoneResidencial, '') <> '' OR ISNULL(M.TelefoneComercial, '') <> '' THEN
	--		CASE WHEN ISNULL(M.TelefoneCelular, '') <> '' THEN ' - Cel.: ' + M.TelefoneCelular ELSE '' END +
	--		CASE WHEN ISNULL(M.TelefoneResidencial, '') <> '' THEN ' - Res.: ' + M.TelefoneResidencial ELSE '' END +
	--		CASE WHEN ISNULL(M.TelefoneComercial, '') <> '' THEN ' - Com.: ' + M.TelefoneComercial ELSE '' END 
	--	ELSE '' END)) AS Telefones, 
	--	Situacao = (SELECT MAX(S.Situacao) FROM SituacaoMembro S WHERE S.MembroId = M.Id AND S.Data = (SELECT MAX(X.Data) FROM SituacaoMembro X WHERE X.MembroId = M.Id)),
	--	CASE WHEN ISNULL(M.ABEDABE, 0) = 1 THEN 'Sim' ELSE 'Não' END AS MembroAbedabe
	--FROM 
	--	Membro M
	--	INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
END
