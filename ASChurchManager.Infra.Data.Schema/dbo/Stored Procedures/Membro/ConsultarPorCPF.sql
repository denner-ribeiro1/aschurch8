CREATE PROCEDURE [dbo].[ConsultarPorCPF]
	@Cpf Varchar(15)
AS
BEGIN
SELECT
		M.Id 
		, M.CongregacaoId
		, M.Nome
		, M.Cpf
		, M.RG
		, M.OrgaoEmissor
		, M.DataNascimento
		, M.NomePai
		, M.NomeMae
		, M.EstadoCivil
		, M.Nacionalidade
		, M.NaturalidadeEstado
		, M.NaturalidadeCidade
		, M.Profissao
		, M.TituloEleitorNumero
		, M.TituloEleitorZona
		, M.TituloEleitorSecao
		, M.TelefoneResidencial
		, M.TelefoneCelular
		, M.TelefoneComercial
		, M.Email
		, M.FotoPath
		, M.Logradouro
		, M.Numero
		, M.Complemento
		, M.Bairro
		, M.Cidade
		, M.Estado
		, M.Pais
		, M.Cep
		, M.DataCriacao
		, M.DataAlteracao
		, M.RecebidoPor
		, M.DataRecepcao
		, M.DataBatismoAguas
		, M.BatimoEspiritoSanto
		, M.ABEDABE
		, M.CriadoPorId
		, M.AprovadoPorId
		, M.Status
		, M.TipoMembro
		, M.DataCriacao
		, M.DataAlteracao
		, M.IdConjuge
		, M.DataPrevistaBatismo
		, M.BatismoId
		, M.Senha
		, ISNULL(M.BatismoSituacao, 0) AS SituacaoBatismo
		-------------------------- Congregação
		, CongregacaoNome = ISNULL(C.Nome, '')
		, CongregacaoResponsavelId = ISNULL(C.CongregacaoResponsavelId, 0)
		, M.Sexo
		, M.Escolaridade
		, CASE WHEN ISNULL(H.Id, 0) = 0 THEN 0 ELSE 1 END AS MembroConfirmado
	FROM dbo.Membro M
	LEFT JOIN dbo.HistoricoMembro H ON M.Id = H.Id
	INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
	WHERE 
		(M.Cpf = @Cpf)
END