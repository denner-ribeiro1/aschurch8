CREATE PROCEDURE dbo.RelatorioCasamentos
	@DataInicio DATETIMEOFFSET, 
	@DataFinal DATETIMEOFFSET,
	@Congregacao INT,
	@UsuarioID INT
AS
BEGIN
	SELECT 
		CA.Id
		,CASE WHEN ISNULL(CA.PastorId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.PastorId) ELSE CA.PastorNome END AS PastorNome
		,CONVERT(VARCHAR, CA.DataHoraInicio, 103) + ' ' + LEFT(convert(varchar, DataHoraInicio,108), 5) + ' - ' + LEFT(convert(varchar, DataHoraFinal,108), 5) AS DataHora 
		,CASE WHEN ISNULL(CA.NoivoId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.NoivoId) ELSE CA.NoivoNome END AS NoivoNome
		,CASE WHEN ISNULL(CA.PaiNoivoId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.PaiNoivoId) ELSE CA.PaiNoivoNome END AS PaiNoivoNome
		,CASE WHEN ISNULL(CA.MaeNoivoId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.MaeNoivoId) ELSE CA.MaeNoivoNome END AS MaeNoivoNome
		,CASE WHEN ISNULL(CA.NoivaId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.NoivaId) ELSE CA.NoivaNome END AS NoivaNome
		,CASE WHEN ISNULL(CA.PaiNoivaId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.PaiNoivaId) ELSE CA.PaiNoivaNome END AS PaiNoivaNome
		,CASE WHEN ISNULL(CA.MaeNoivaId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.MaeNoivaId) ELSE CA.MaeNoivaNome END AS MaeNoivaNome
		,ISNULL(C.Nome, '') AS CongregacaoNome 
	FROM 
		dbo.Casamento CA
	INNER JOIN dbo.Congregacao C ON CA.CongregacaoId = C.Id
	WHERE
		(@DataInicio IS NULL OR (CONVERT(DATE, CA.DataHoraInicio) >= CONVERT(DATE, @DataInicio) AND CONVERT(DATE, CA.DataHoraInicio) <= CONVERT(DATE, @DataFinal) ))
		AND (ISNULL(@Congregacao, 0) = 0 OR CA.CongregacaoId = @Congregacao)
		AND (ISNULL(@UsuarioID,0) = 0 OR C.Id IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default)))
	ORDER BY
		CA.DataHoraInicio
END
