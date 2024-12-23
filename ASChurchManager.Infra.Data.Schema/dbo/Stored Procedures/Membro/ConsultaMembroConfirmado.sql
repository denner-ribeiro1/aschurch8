CREATE PROCEDURE [dbo].[ConsultaMembroConfirmado]
	@membroId int
AS
BEGIN
	SELECT 
		H.Id
		, H.Controle
		, H.Nome
		, H.Cpf
		, H.RG
		, H.OrgaoEmissor
		, H.DataNascimento
		, H.Sexo
		, H.Escolaridade
		, H.NomePai
		, H.NomeMae
		, H.EstadoCivil
		, H.Nacionalidade
		, H.NaturalidadeEstado
		, H.NaturalidadeCidade
		, H.Profissao
		, H.TelefoneResidencial
		, H.TelefoneCelular
		, H.TelefoneComercial
		, H.Email
		, H.Logradouro
		, H.Numero
		, H.Complemento
		, H.Bairro
		, H.Cidade
		, H.Estado
		, H.Pais
		, H.Cep
		, H.DataAlteracao
		, [IP]
	FROM
		HistoricoMembro H
	WHERE
		H.Id = @membroId
		AND H.Controle = (SELECT MAX(X.Controle) FROM HistoricoMembro X WHERE X.Id = H.Id)

	SELECT 
		Id
		, Nome
		, Cpf
		, RG
		, OrgaoEmissor
		, DataNascimento
		, Sexo
		, Escolaridade
		, NomePai
		, NomeMae
		, EstadoCivil
		, Nacionalidade
		, NaturalidadeEstado
		, NaturalidadeCidade
		, Profissao
		, TelefoneResidencial
		, TelefoneCelular
		, TelefoneComercial
		, Email
		, Logradouro
		, Numero
		, Complemento
		, Bairro
		, Cidade
		, Estado
		, Pais
		, Cep
		, DataAlteracao
	FROM
		Membro
	WHERE
		Id = @membroId
END
