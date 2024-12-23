CREATE PROCEDURE [dbo].[RelPresencaLista]
	@PresencaId INT,
	@DataId INT,
	@UsuarioId INT
AS
BEGIN
	SELECT DISTINCT
		P.Id,
		P.Descricao,
		PD.DataHoraInicio,
		PD.DataHoraFim,
		ISNULL(PI.MembroId, 0) AS MembroId, 
		ISNULL(PI.Nome, M.Nome) AS Nome,
		ISNULL(PI.CPF, M.CPF) AS CPF,
		ISNULL(ISNULL(CCO.Id, M.CongregacaoId), 0) AS CongregacaoId,
		CASE WHEN PI.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE PI.Igreja END AS Igreja,
		CASE WHEN PI.MembroId > 0 THEN ISNULL(CA.Descricao, 'Membro') ELSE ISNULL(PI.Cargo, 'Visitante') END AS Cargo,
		ISNULL(PI.Cargo, CA.Descricao) AS Cargo,
		PID.Situacao,
		PID.Justificativa
	FROM Presenca P
	INNER JOIN PresencaDatas PD ON PD.PresencaId = P.Id
	INNER JOIN PresencaInscricao PI ON PI.PresencaId = P.Id
	INNER JOIN PresencaInscricaoDatas PID ON PID.DataId = PD.Id AND PID.InscricaoId = PI.Id
	LEFT JOIN Membro M ON M.Id = PI.MembroId
	LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id
	LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id)
	LEFT JOIN Cargo CA ON CM.CargoId = CA.Id 
	LEFT JOIN CongregacaoObreiro CO ON CO.MembroId = M.Id 
	LEFT JOIN Congregacao CCO ON CCO.Id = CO.CongregacaoId 
	WHERE 
		P.Id = @PresencaId 
		AND (@DataId = 0 OR PD.Id = @DataId)
		AND (ISNULL(M.CongregacaoId, 0) = 0 OR C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)))
	ORDER BY   
		PD.DataHoraInicio, 
		ISNULL(PI.Nome, M.Nome),
		P.Id,
		P.Descricao,
		PD.DataHoraFim,
		ISNULL(PI.MembroId, 0), 
		ISNULL(PI.CPF, M.CPF),
		ISNULL(ISNULL(CCO.Id, M.CongregacaoId), 0),
		CASE WHEN PI.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE PI.Igreja END,
		CASE WHEN PI.MembroId > 0 THEN ISNULL(CA.Descricao, 'Membro') ELSE ISNULL(PI.Cargo, 'Visitante') END,
		ISNULL(PI.Cargo, CA.Descricao),
		PID.Situacao
END
