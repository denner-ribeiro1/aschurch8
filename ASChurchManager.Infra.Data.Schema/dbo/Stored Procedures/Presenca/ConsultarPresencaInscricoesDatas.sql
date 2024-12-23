CREATE PROCEDURE [dbo].[ConsultarPresencaInscricoesDatas]
	@PresencaId INT,
	@DataId INT
AS
BEGIN
	IF OBJECT_ID('TEMPDB..#INSCR') IS NOT NULL DROP TABLE #INSCR
	SELECT
		I.InscricaoId AS Id,
		D.PresencaId, 
		I.DataId,
		I.Situacao,
		I.Tipo,
		I.Justificativa
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
		'' AS Justificativa
	FROM
		PresencaInscricao P
	WHERE 
		P.PresencaId = @PresencaId
		AND NOT EXISTS(SELECT TOP 1 1 FROM #INSCR X WHERE X.Id = P.Id) 

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
		P.Justificativa
	FROM  
		#INSCR P
	INNER JOIN PresencaInscricao PI ON P.Id = PI.Id
	LEFT JOIN Membro M ON M.Id = PI.MembroId
	LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id
	LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id)
	LEFT JOIN Cargo CA ON CM.CargoId = CA.Id 
	LEFT JOIN CongregacaoObreiro CO ON CO.MembroId = M.Id 
	LEFT JOIN Congregacao CCO ON CCO.Id = CO.CongregacaoId 
END