
CREATE PROCEDURE [dbo].[ConsultarCongregacao]
	@Id INT
	, @UsuarioID INT
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
		, C.Pais
		, C.Cep
		, C.DataCriacao
		, C.DataAlteracao
		, C.Sede
		, CongregacaoResponsavelId = ISNULL(C.CongregacaoResponsavelId, @SedeId)
		, CongregacaoResponsavelNome = (SELECT X.Nome FROM dbo.Congregacao X
										WHERE X.Id = ISNULL(C.CongregacaoResponsavelId, @SedeId))
		, C.PastorResponsavelId
		, PastorResponsavelNome = (SELECT M.Nome FROM dbo.Membro M
								   WHERE M.Id = C.PastorResponsavelId)
		, C.CNPJ
	FROM 
		dbo.Congregacao C
	WHERE
		Id = @Id
		AND C.Situacao = 1
		AND C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
END