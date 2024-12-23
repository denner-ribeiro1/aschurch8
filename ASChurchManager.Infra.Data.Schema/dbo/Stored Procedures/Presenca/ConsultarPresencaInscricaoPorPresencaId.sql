CREATE PROCEDURE [dbo].[ConsultarPresencaInscricaoPorPresencaId]
	@PresencaId INT,
	@UsuarioId INT
AS
BEGIN
	SELECT
		P.Id,
		P.PresencaId,
		P.MembroId,
		CASE WHEN P.MembroId > 0 THEN M.Nome ELSE P.Nome END AS Nome,
		CASE WHEN P.MembroId > 0 THEN M.CPF ELSE P.CPF END AS CPF,
		M.CongregacaoId AS CongregacaoId,
		CASE WHEN P.MembroId > 0 THEN ISNULL(CCO.Nome, C.Nome) ELSE P.Igreja END AS Igreja,
		CASE WHEN P.MembroId > 0 THEN ISNULL(CA.Descricao, 'Membro') ELSE CASE WHEN P.Cargo IS NULL THEN 'Visitante' ELSE P.Cargo + ' - Visitante' END END AS Cargo,
		P.Pago,
		P.DataCriacao,
		P.DataAlteracao,
		P.Usuario
	FROM
		PresencaInscricao P
		INNER JOIN Usuario U ON U.Id = P.Usuario
		LEFT JOIN Membro M ON M.Id = P.MembroId
		LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id
		LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id)
		LEFT JOIN Cargo CA ON CM.CargoId = CA.Id 
		LEFT JOIN CongregacaoObreiro CO ON CO.MembroId = M.Id
		LEFT JOIN Congregacao CCO ON CCO.Id = CO.CongregacaoId
 	WHERE
		PresencaId =  @PresencaId
		AND U.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioId, default))
	ORDER BY
		6, 4
END
