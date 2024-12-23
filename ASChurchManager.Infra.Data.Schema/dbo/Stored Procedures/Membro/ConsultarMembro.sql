CREATE PROCEDURE [dbo].[ConsultarMembro]
	@Id INT
	, @UsuarioID INT
AS
BEGIN
	
	SET NOCOUNT ON
	
	SELECT
		M.Id 
		, M.CongregacaoId
		, M.Nome
		, M.Cpf
		, M.RG
		, M.OrgaoEmissor
		, M.DataNascimento
		, M.IdPai
		, CASE WHEN ISNULL(M.IdPai, 0) > 0 THEN MP.Nome ELSE M.NomePai END AS NomePai
		, M.IdMae
		, CASE WHEN ISNULL(M.IdMae, 0) > 0 THEN MM.Nome ELSE M.NomeMae END AS NomeMae
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
		, AprovadoPorNome = ISNULL(U2.Nome,'')
		, M.IdConjuge
		, CASE WHEN ISNULL(M.IdConjuge, 0) > 0 THEN M2.Nome ELSE M.NomeConjuge END AS NomeConjuge
		, M.DataPrevistaBatismo
		, M.BatismoId
		, ISNULL(M.BatismoSituacao, 0) AS SituacaoBatismo
		-------------------------- Congregação
		, CongregacaoNome = ISNULL(C.Nome,'')
		, CriadoPorNome = ISNULL(U1.Nome,'')
		, HA.MotivoReprovacao
		, UsuarioReprovacao = U3.Nome
		, M.Sexo
		, M.Escolaridade
		, CASE WHEN ISNULL(H.Id, 0) = 0 THEN 0 ELSE 1 END AS MembroConfirmado
		, M.FotoUrl
		, M.TamanhoCapa
	FROM dbo.Membro M
	LEFT JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
	LEFT JOIN dbo.Usuario U1 ON M.CriadoPorId = U1.id
	LEFT JOIN dbo.Usuario U2 ON M.AprovadoPorId = U2.id
	LEFT JOIN dbo.Membro M2 ON M.IdConjuge = M2.Id
	LEFT JOIN dbo.HistoricoAprovacaoMembro HA ON M.Id = HA.MembroId AND HA.StatusAprovacao = 'P'
	LEFT JOIN dbo.Usuario U3 ON HA.UsuarioId = U3.Id
	LEFT JOIN dbo.HistoricoMembro H ON M.Id = H.Id
	LEFT JOIN dbo.Membro MP ON M.IdPai = MP.Id
	LEFT JOIN dbo.Membro MM ON M.IdMae = MM.Id
	WHERE 
		M.Id = @Id
		AND M.CongregacaoId IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
END