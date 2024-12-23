create PROCEDURE dbo.ConsultarNascimento
	@Id INT
   ,@UsuarioID INT
AS
BEGIN
	SELECT 
		Na.Id, 
		Na.Crianca,
		Na.Sexo,
		Na.CongregacaoID,
		Na.IdMembroPai,
		CASE WHEN ISNULL(Na.IdMembroPai,0) > 0 THEN (SELECT Nome from Membro where Id = Na.IdMembroPai) else Na.NomePai end as NomePai,
		Na.IdMembroMae,
		CASE WHEN ISNULL(Na.IdMembroMae,0) > 0 THEN (SELECT Nome from Membro where Id = Na.IdMembroMae) else Na.NomeMae end as NomeMae,
		Na.DataApresentacao,
		Na.DataNascimento,
		Na.DataCriacao,
		Na.DataAlteracao,
		Na.PastorId,
		CASE WHEN ISNULL(Na.PastorId,0) > 0 THEN (SELECT Nome from Membro where Id = Na.PastorId) else Na.Pastor end as Pastor,
		ISNULL(C.Nome,'') AS CongregacaoNome,
		C.CongregacaoResponsavelId
	FROM 
		dbo.Nascimento Na
		LEFT JOIN dbo.Congregacao C ON Na.CongregacaoId = C.Id
	WHERE
		Na.Id = @Id
		AND C.Id IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
END