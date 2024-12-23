CREATE PROCEDURE [dbo].[ListarCasamento]
	@UsuarioID INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT 
		CA.Id
		,CA.CongregacaoId
		,CA.PastorId
		,CASE WHEN ISNULL(CA.PastorId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.PastorId) ELSE CA.PastorNome END AS PastorNome
		,CA.DataHoraInicio
		,CA.DataHoraFinal
		,CA.NoivoId
		,CASE WHEN ISNULL(CA.NoivoId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.NoivoId) ELSE CA.NoivoNome END AS NoivoNome
		,CA.PaiNoivoId
		,CASE WHEN ISNULL(CA.PaiNoivoId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.PaiNoivoId) ELSE CA.PaiNoivoNome END AS PaiNoivoNome
		,CA.MaeNoivoId
		,CASE WHEN ISNULL(CA.MaeNoivoId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.MaeNoivoId) ELSE CA.MaeNoivoNome END AS MaeNoivoNome
		,CA.NoivaId
		,CASE WHEN ISNULL(CA.NoivaId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.NoivaId) ELSE CA.NoivaNome END AS NoivaNome
		,CA.PaiNoivaId
		,CASE WHEN ISNULL(CA.PaiNoivaId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.PaiNoivaId) ELSE CA.PaiNoivaNome END AS PaiNoivaNome
		,CA.MaeNoivaId
		,CASE WHEN ISNULL(CA.MaeNoivaId, 0) > 0 THEN (SELECT Nome FROM Membro WHERE Id = CA.MaeNoivaId) ELSE CA.MaeNoivaNome END AS MaeNoivaNome
		,CA.DataCriacao
		,CA.DataAlteracao
		,ISNULL(C.Nome,'') AS CongregacaoNome 
		,C.CongregacaoResponsavelId
	FROM 
		dbo.Casamento CA
	LEFT JOIN dbo.Congregacao C ON CA.CongregacaoId = C.Id
	WHERE 
		C.Id IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
END