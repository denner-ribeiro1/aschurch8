CREATE PROCEDURE [dbo].[ConsultarPresencaInscricaoIdCPF]
	@PresencaId INT,
	@MembroId INT,
	@CPF VARCHAR(15),
	@UsuarioId INT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE 
		@SQLQUERY NVARCHAR(4000),
		@PARMDEFINITION NVARCHAR(500)

	SET @SQLQUERY =
		'SELECT '+
		'	P.Id, '+
		'	P.PresencaId, '+
		'	P.MembroId, '+
		'	ISNULL(P.Nome, M.Nome) AS Nome, '+
		'	ISNULL(P.CPF, M.CPF) AS CPF, '+
		'	M.CongregacaoId AS CongregacaoId, '+
		'	ISNULL(P.Igreja, C.Nome) AS Igreja, '+
		'	ISNULL(P.Cargo, CA.Descricao) AS Cargo, '+
		'	P.Pago, '+
		'	P.DataCriacao, '+
		'	P.DataAlteracao, '+
		'	P.Usuario '+
		'FROM '+
		'	PresencaInscricao P '+
		'	LEFT JOIN Membro M ON M.Id = P.MembroId '+
		'	LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id '+
		'	LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id) '+
		'	LEFT JOIN Cargo CA ON CM.CargoId = CA.Id  '+
		'WHERE ' +
		'	P.PresencaId =  @PresencaId ' + 
		'	AND (ISNULL(M.CongregacaoId, 0) = 0 OR M.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioId, default)))'
	
	IF (@MembroId > 0)
		SET @SQLQUERY += '	AND P.MembroId = @MembroId '
	
	IF (ISNULL(@CPF, '') <> '')
		SET @SQLQUERY += '	AND P.CPF = @CPF '

	SET @SQLQUERY += 'ORDER BY C.Id, M.Nome'

	/* EXECUCAÇÃO DA QUERY */
	SET @PARMDEFINITION = '@PresencaId INT, @MembroId INT, @CPF VARCHAR(15), @UsuarioId INT'

	EXECUTE SP_EXECUTESQL @SQLQUERY, @PARMDEFINITION, @PresencaId, @MembroId, @CPF, @UsuarioId 
END
