CREATE PROCEDURE [dbo].[SelecionaMembrosParaBatismo]
	@BatismoId INT,
	@Status INT,
	@UsuarioID INT
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
	FROM
		Membro M
	INNER JOIN Batismo B ON M.BatismoId = B.Id
	INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
	WHERE
		(ISNULL(@BatismoId, 0) = 0 OR B.Id = @BatismoId)
		AND (ISNULL(@Status, 0) = 0 OR B.Status = @Status)
		AND M.CongregacaoId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
		AND M.Status = 1 --ATIVO
	ORDER BY
		M.CongregacaoId, M.Id
END