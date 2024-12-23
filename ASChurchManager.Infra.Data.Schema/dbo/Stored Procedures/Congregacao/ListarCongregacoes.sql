
CREATE PROCEDURE [dbo].[ListarCongregacoes]
	@UsuarioID INT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SedeId INT = 0

	SELECT 
		@SedeId = Id 
	FROM 
		dbo.Congregacao
	WHERE 
		Sede = 1

	SELECT 
		C.Id
		, C.Nome
		, C.Logradouro
		, C.Numero
		, C.Complemento
		, C.Bairro
		, C.Cidade
		, C.Estado
		, C.Cep
		, C.Pais
		, C.DataCriacao
		, C.DataAlteracao
		, C.Sede
		, CongregacaoResponsavelId = ISNULL(CongregacaoResponsavelId, @SedeId)
		, CongregacaoResponsavelNome = (SELECT Nome FROM dbo.Congregacao 
										WHERE Id = ISNULL(C.CongregacaoResponsavelId, @SedeId))
		, C.PastorResponsavelId
		, PastorResponsavelNome = (SELECT M.Nome FROM dbo.Membro M
								   WHERE M.Id = C.PastorResponsavelId)
		, C.CNPJ
	FROM 
		dbo.Congregacao C
	WHERE
		C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
		AND C.Situacao = 1
END