CREATE PROCEDURE [dbo].[ExisteCPFDuplicadoMembro]
	@MembroId Int,
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
		, ISNULL(M.BatismoSituacao, 0) AS SituacaoBatismo
		-------------------------- Congregação
		, CongregacaoNome = ISNULL(C.Nome,'')
		, M.Sexo
		, M.Escolaridade
	FROM dbo.Membro M
	INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
	WHERE 
		((@MembroId = 0 or M.Id <> @MembroId) And M.Cpf = @Cpf)
END