CREATE PROCEDURE dbo.ConsultarDadosMembroFichaMembro
	@Id INT	
AS
BEGIN
	SELECT
		M.Id
		, CONVERT(VARCHAR, M.CongregacaoId) + ' - ' + C.Nome As Congregacao
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
		, M.Sexo
		, M.Escolaridade
		, M.Profissao
		, M.TituloEleitorNumero
		, M.TituloEleitorZona
		, M.TituloEleitorSecao
		, M.TelefoneResidencial
		, M.TelefoneCelular
		, M.TelefoneComercial
		, M.Email
		, M.FotoPath
		, REPLACE(M.FotoPath, 'data:image/png;base64,', '') AS FotoByte
		, M.Logradouro
		, M.Numero
		, M.Complemento
		, M.Bairro
		, M.Cidade
		, M.Estado
		, M.Pais
		, M.Cep
		, M.RecebidoPor
		, M.DataRecepcao
		, M.DataBatismoAguas
		, CASE WHEN M.BatimoEspiritoSanto = 1 THEN 'SIM' ELSE 'NÃO' END AS BatimoEspiritoSanto
		, CASE WHEN M.ABEDABE = 1 THEN 'SIM' ELSE 'NÃO' END AS ABEDABE
		, M.Status
		, M.TipoMembro
		, M.IdConjuge
		, CASE WHEN ISNULL(M.IdConjuge, 0) > 0 THEN MC.Nome ELSE M.NomeConjuge END AS NomeConjuge
		, CASE WHEN ISNULL(M.ABEDABE, 0) = 1 THEN 'SIM' ELSE 'NÃO' END AS MembroAbedabe
		, U1.Nome AS UsuarioCriacao
		, U2.Nome AS UsuarioAprovacao
		, M.DataCriacao
		, M.DataAlteracao
		, M.FotoUrl
	FROM 
		Membro M
		INNER JOIN Congregacao C ON C.Id = M.CongregacaoId
		LEFT JOIN Membro MC ON M.IdConjuge = MC.Id
		LEFT JOIN Membro MP ON M.IdPai = MP.Id
		LEFT JOIN Membro MM ON M.IdMae = MM.Id
		LEFT JOIN Usuario U1 ON U1.Id = M.CriadoPorId
		LEFT JOIN Usuario U2 ON U2.Id = M.AprovadoPorId
	WHERE
		M.Id = @Id
END