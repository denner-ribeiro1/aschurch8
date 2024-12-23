
CREATE PROCEDURE [dbo].[SalvarMembro]
	@Id INT = 0
	, @CongregacaoId INT
	, @Nome varchar(100)
	, @Cpf varchar(15)
	, @RG varchar(20) = NULL
	, @OrgaoEmissor varchar(20) = NULL
	, @DataNascimento DATETIMEOFFSET = NULL
	, @NomePai varchar(100) = NULL
	, @NomeMae varchar(100) = NULL
	, @EstadoCivil int = NULL
	, @Sexo TINYINT = NULL
	, @Escolaridade TINYINT = NULL
	, @Nacionalidade varchar(50) = NULL
	, @NaturalEstado varchar(2) = NULL
	, @NaturalCidade varchar(50) = NULL
	, @Profissao varchar(100) = NULL
	, @TituloEleitorNumero varchar(30) = NULL
	, @TituloEleitorZona varchar(30) = NULL
	, @TituloEleitorSecao varchar(30) = NULL
	, @TelefoneResidencial varchar(15) = NULL
	, @TelefoneCelular varchar(15) = NULL
	, @TelefoneComercial varchar(15) = NULL
	, @Email varchar(100) = NULL
	, @FotoPath VARCHAR(MAX) = NULL
	, @Logradouro VARCHAR(100) = NULL
	, @Numero VARCHAR(10) = NULL
	, @Complemento VARCHAR(100) = NULL
	, @Bairro VARCHAR(100) = NULL
	, @Cidade VARCHAR(100) = NULL
	, @Estado VARCHAR(30) = NULL
	, @Pais VARCHAR(100) = NULL
	, @Cep VARCHAR(15) = NULL
	, @RecebidoPor INT = NULL
	, @DataRecepcao DATETIMEOFFSET = NULL
	, @DataBatismoAguas DATETIMEOFFSET = NULL
	, @BatimoEspiritoSanto BIT = NULL
	, @ABEDABE BIT = NULL
	, @Status INT = 0
	, @TipoMembro INT = NULL
	, @IdConjuge INT = NULL
	, @BatismoId INT = NULL
	, @CriadoPorId INT = NULL
	, @AprovadoPorId INT = NULL
	, @TamanhoCapa TINYINT = NULL
	, @IdPai INT = NULL
	, @IdMae INT = NULL
	, @NomeConjuge VARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @ERRO VARCHAR(8000)
	/*TRATAMENTO UTILIZADO PARA O BATISMO*/
	DECLARE @DataPrevistaBatismo DATETIMEOFFSET
	DECLARE @SitAtual INT
	IF (@TipoMembro = 2 AND @BatismoId > 0)
	BEGIN
		SELECT
			@DataPrevistaBatismo = DataBatismo
		FROM
			Batismo
		WHERE 
			Id = @BatismoId
	END

	IF EXISTS (SELECT TOP 1 1 FROM dbo.Membro WHERE Id = @Id)
	BEGIN
		BEGIN TRAN	
		BEGIN TRY
			-- UPDATE
			UPDATE 
				M
			SET 
				CongregacaoId = @CongregacaoId
				, Nome = @Nome
				, Cpf = @Cpf
				, RG = @RG
				, OrgaoEmissor = @OrgaoEmissor
				, DataNascimento = @DataNascimento
				, IdPai = @IdPai
				, NomePai = @NomePai
				, IdMae = @IdMae
				, NomeMae = @NomeMae
				, EstadoCivil = @EstadoCivil
				, Nacionalidade = @Nacionalidade
				, NaturalidadeEstado = @NaturalEstado
				, NaturalidadeCidade = @NaturalCidade
				, Profissao = @Profissao
				, TituloEleitorNumero = @TituloEleitorNumero
				, TituloEleitorZona = @TituloEleitorZona
				, TituloEleitorSecao = @TituloEleitorSecao
				, TelefoneResidencial = @TelefoneResidencial
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
				, RecebidoPor = @RecebidoPor
				, DataRecepcao = @DataRecepcao
				, DataBatismoAguas = @DataBatismoAguas
				, BatimoEspiritoSanto = @BatimoEspiritoSanto
				, ABEDABE = @ABEDABE
				, Status = @Status
				, TipoMembro = @TipoMembro
				, DataAlteracao = CONVERT(VARCHAR, GETDATE(),112)
				, FotoPath = @FotoPath
				, IdConjuge = @IdConjuge
				, NomeConjuge = @NomeConjuge
				, DataPrevistaBatismo = @DataPrevistaBatismo
				, BatismoId = @BatismoId
				, Sexo = @Sexo
				, Escolaridade = @Escolaridade
				, TamanhoCapa = @TamanhoCapa
			FROM 
				dbo.Membro M
			WHERE 
				Id = @Id

			/*ATUALIZANDO O CONJUGE*/
			IF (ISNULL(@IdConjuge, 0) > 0)
			BEGIN
				UPDATE
					Membro
				SET
					IdConjuge = @Id
					, EstadoCivil = @EstadoCivil
				WHERE 
					Id = @IdConjuge
			END

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			SELECT @ERRO = 'Atualizando Membro - Código: ' + CONVERT(VARCHAR(6), ERROR_NUMBER()) + ' - ' + ERROR_MESSAGE()
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;
			RAISERROR (15600,-1,-1, @ERRO);
		END CATCH
	END
	ELSE 
	BEGIN	
		BEGIN TRAN	
		BEGIN TRY
			DECLARE @UltMembro INT 
			SELECT @UltMembro = ISNULL(MAX(Id),0) + 1 FROM dbo.Membro 
			
			INSERT INTO dbo.Membro
			(
				Id
				, CongregacaoId
				, Nome
				, Cpf
				, RG
				, OrgaoEmissor
				, DataNascimento
				, IdPai
				, NomePai
				, IdMae
				, NomeMae
				, EstadoCivil
				, Nacionalidade
				, NaturalidadeEstado
				, NaturalidadeCidade
				, Profissao
				, TituloEleitorNumero
				, TituloEleitorZona
				, TituloEleitorSecao
				, TelefoneResidencial
				, TelefoneCelular
				, TelefoneComercial
				, Email
				, FotoPath
				, Logradouro
				, Numero
				, Complemento
				, Bairro
				, Cidade
				, Estado
				, Pais
				, Cep
				, RecebidoPor
				, DataRecepcao
				, DataBatismoAguas
				, BatimoEspiritoSanto
				, ABEDABE
				, CriadoPorId
				, AprovadoPorId
				, Status
				, TipoMembro
				, DataCriacao
				, DataAlteracao
				, IdConjuge
				, NomeConjuge
				, DataPrevistaBatismo
				, BatismoId
				, Sexo
				, Escolaridade
				, TamanhoCapa
			)
			SELECT
				@UltMembro
				, @CongregacaoId
				, @Nome
				, @Cpf
				, @RG
				, @OrgaoEmissor
				, @DataNascimento
				, @IdPai
				, @NomePai
				, @IdMae
				, @NomeMae
				, @EstadoCivil
				, @Nacionalidade
				, @NaturalEstado
				, @NaturalCidade
				, @Profissao
				, @TituloEleitorNumero
				, @TituloEleitorZona
				, @TituloEleitorSecao
				, @TelefoneResidencial
				, @TelefoneCelular
				, @TelefoneComercial
				, @Email
				, @FotoPath
				, @Logradouro
				, @Numero
				, @Complemento
				, @Bairro
				, @Cidade
				, @Estado
				, @Pais
				, @Cep
				, @RecebidoPor
				, @DataRecepcao
				, @DataBatismoAguas
				, @BatimoEspiritoSanto
				, @ABEDABE
				, @CriadoPorId
				, @AprovadoPorId
				, @Status
				, @TipoMembro
				, CONVERT(VARCHAR, GETDATE(),112)
				, CONVERT(VARCHAR, GETDATE(),112)
				, @IdConjuge
				, @NomeConjuge
				, @DataPrevistaBatismo
				, @BatismoId
				, @Sexo
				, @Escolaridade
				, @TamanhoCapa


			SELECT @Id = @UltMembro

			/*ATUALIZANDO O CONJUGE*/
			IF (ISNULL(@IdConjuge, 0) > 0)
			BEGIN
				UPDATE
					Membro
				SET
					IdConjuge = @Id
					, EstadoCivil = @EstadoCivil
				WHERE 
					Id = @IdConjuge
			END
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			SELECT @ERRO = 'Salvando Membro - Código: ' + CONVERT(VARCHAR(6), ERROR_NUMBER()) + ' - ' + ERROR_MESSAGE()
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;
			RAISERROR (15600,-1,-1, @ERRO);
		END CATCH
	END
	SELECT @Id
	RETURN @Id
END