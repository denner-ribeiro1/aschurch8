CREATE PROCEDURE [dbo].[ConsultarPresencaInscricaoDatasPorIdEData]
	@InscricaoId INT,
	@DataId INT
AS
BEGIN
	SELECT
		P.Id,
		P.PresencaId,
		P.MembroId,
		ISNULL(P.Nome, M.Nome) AS Nome,
		ISNULL(P.CPF, M.CPF) AS CPF,
		M.CongregacaoId AS CongregacaoId,
		ISNULL(P.Igreja, C.Nome) AS Igreja,
		ISNULL(P.Cargo, CA.Descricao) AS Cargo,
		P.Pago,
		P.DataCriacao,
		P.DataAlteracao,
		P.Usuario,
		D.Situacao AS Situacao,
		D.Tipo
	FROM
		PresencaInscricao P
		INNER JOIN PresencaInscricaoDatas D ON D.InscricaoId = P.Id
		LEFT JOIN Membro M ON M.Id = P.MembroId
		LEFT JOIN Congregacao C ON M.CongregacaoId = C.Id
		LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id)
		LEFT JOIN Cargo CA ON CM.CargoId = CA.Id 
	WHERE 
		P.Id = @InscricaoId 
		AND D.DataId = @DataId 
END