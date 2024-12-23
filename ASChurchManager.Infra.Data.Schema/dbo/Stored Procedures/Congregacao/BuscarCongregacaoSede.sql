CREATE PROCEDURE BuscarCongregacaoSede
AS
BEGIN
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
		, CongregacaoResponsavelId = C.CongregacaoResponsavelId
		, CongregacaoResponsavelNome = (SELECT X.Nome FROM dbo.Congregacao X WHERE X.Id = C.CongregacaoResponsavelId)
		, C.PastorResponsavelId
		, PastorResponsavelNome = (SELECT M.Nome FROM dbo.Membro M WHERE M.Id = C.PastorResponsavelId)
		, C.CNPJ
	FROM 
		dbo.Congregacao C
	WHERE 
		Sede = 1		
END