CREATE PROCEDURE [dbo].[AtualizarMembroExterno]
	@Id INT
	--, @Nome varchar(100)
	--, @Cpf varchar(15)
	--, @RG varchar(20) = NULL
	--, @OrgaoEmissor varchar(20) = NULL
	--, @DataNascimento DATETIMEOFFSET = NULL
	--, @NomePai varchar(100) = NULL
	--, @NomeMae varchar(100) = NULL
	--, @EstadoCivil int = NULL
	--, @Sexo TINYINT = NULL
	--, @Escolaridade TINYINT = NULL
	--, @Nacionalidade varchar(50) = NULL
	--, @NaturalEstado varchar(2) = NULL
	--, @NaturalCidade varchar(50) = NULL
	--, @Profissao varchar(100) = NULL
	, @TelefoneResidencial varchar(15) = NULL
	, @TelefoneCelular varchar(15) = NULL
	, @TelefoneComercial varchar(15) = NULL
	, @Email varchar(100) = NULL
	, @Logradouro VARCHAR(100) = NULL
	, @Numero VARCHAR(10) = NULL
	, @Complemento VARCHAR(100) = NULL
	, @Bairro VARCHAR(100) = NULL
	, @Cidade VARCHAR(100) = NULL
	, @Estado VARCHAR(2) = NULL
	, @Pais VARCHAR(100) = NULL
	, @Cep VARCHAR(10) = NULL
	, @Ip Varchar(500)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @ERRO VARCHAR(8000)

	BEGIN TRAN	

	BEGIN TRY
		DECLARE @UltControle INT 
		SELECT @UltControle = ISNULL(MAX(Controle),0) + 1 FROM dbo.HistoricoMembro
		WHERE Id = @id

		INSERT INTO dbo.HistoricoMembro
		(
			Id
			, Controle
			, Nome
			, Cpf
			, RG
			, OrgaoEmissor
			, DataNascimento
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
			, Sexo
			, Escolaridade
			, DataAlteracao
			, [Ip]
		)
		SELECT
			Id
			, @UltControle
			, Nome
			, Cpf
			, RG
			, OrgaoEmissor
			, DataNascimento
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
			, Sexo
			, Escolaridade
			, GETDATE()
			, @Ip
		From
			Membro
		Where
			Id = @id

		-- UPDATE
		UPDATE 
			M
		SET 
			--Nome = @Nome
			--, Cpf = @Cpf
			--, RG = @RG
			--, OrgaoEmissor = @OrgaoEmissor
			--, DataNascimento = @DataNascimento
			--, NomePai = @NomePai
			--, NomeMae = @NomeMae
			--, EstadoCivil = @EstadoCivil
			--, Nacionalidade = @Nacionalidade
			--, NaturalidadeEstado = @NaturalEstado
			--, NaturalidadeCidade = @NaturalCidade
			--, Profissao = @Profissao				
			TelefoneResidencial = @TelefoneResidencial
			, TelefoneCelular = @TelefoneCelular
			, TelefoneComercial = @TelefoneComercial
			, Email = @Email
			, Logradouro = @Logradouro
			, Numero = @Numero
			, Complemento = @Complemento
			, Bairro = @Bairro
			, Cidade = @Cidade
			, Estado = @Estado
			, Pais = @Pais
			, Cep = @Cep
			--, Sexo = @Sexo
			--, Escolaridade = @Escolaridade
		FROM 
			dbo.Membro M
		WHERE 
			Id = @Id

		COMMIT TRAN
	END TRY
	BEGIN CATCH
			SELECT @ERRO = 'Atualizando Membro - Código: ' + CONVERT(VARCHAR(6), ERROR_NUMBER()) + ' - ' + ERROR_MESSAGE()
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;
			RAISERROR (15600,-1,-1, @ERRO);
	END CATCH
	SELECT @Id
	RETURN @Id	
END