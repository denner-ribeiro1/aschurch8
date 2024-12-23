CREATE PROC AtualizarCodigoRegistroMembro
	@IdAntigo int,
	@IdNovo Int
AS
BEGIN
	PRINT 'Membro'
	SELECT * FROM Membro WHERE Id = @IdAntigo
	PRINT 'CargoMembro'
	SELECT * FROM CargoMembro WHERE MembroId = @IdAntigo
	PRINT 'ObservacaoMembro'
	SELECT * FROM ObservacaoMembro WHERE MembroId = @IdAntigo
	PRINT 'SituacaoMembro'
	SELECT * FROM SituacaoMembro WHERE MembroId = @IdAntigo

	BEGIN TRY 
		BEGIN TRAN
	
		IF (EXISTS(SELECT TOP 1 * FROM Membro WHERE Id = @IdNovo))
		BEGIN 
			RAISERROR ('Id Membro Novo já existe', -- Message text.
			   16, -- Severity.
			   1 -- State.
			   );
		END

		SET IDENTITY_INSERT dbo.Membro ON
		INSERT INTO dbo.Membro (
			Id
			, CongregacaoId
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
			, DataPrevistaBatismo
			, BatimoEspiritoSanto
			, CriadoPorId
			, AprovadoPorId
			, Status
			, TipoMembro
			, DataCriacao
			, DataAlteracao
			, IdConjuge
		)
		SELECT
			@IdNovo
			, CongregacaoId
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
			, DataPrevistaBatismo
			, BatimoEspiritoSanto
			, CriadoPorId
			, AprovadoPorId
			, Status
			, TipoMembro
			, DataCriacao
			, DataAlteracao
			, IdConjuge
		FROM
			Membro
		WHERE
			Id = @IdAntigo

		SET IDENTITY_INSERT dbo.Membro OFF

		UPDATE
			CargoMembro
		SET
			MembroId = @IdNovo
		WHERE
			MembroId = @IdAntigo

		UPDATE
			ObservacaoMembro
		SET
			MembroId = @IdNovo
		WHERE
			MembroId = @IdAntigo

		UPDATE
			SituacaoMembro
		SET
			MembroId = @IdNovo
		WHERE
			MembroId = @IdAntigo
		
		DELETE Membro WHERE Id = @IdAntigo 
		COMMIT
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE(),
			   @ErrorSeverity = ERROR_SEVERITY(),
			   @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
		ROLLBACK TRAN
	END CATCH

	PRINT 'Membro'
	SELECT * FROM Membro WHERE Id = @IdNovo
	PRINT 'CargoMembro'
	SELECT * FROM CargoMembro WHERE MembroId = @IdNovo
	PRINT 'ObservacaoMembro'
	SELECT * FROM ObservacaoMembro WHERE MembroId = @IdNovo
	PRINT 'SituacaoMembro'
	SELECT * FROM SituacaoMembro WHERE MembroId = @IdNovo
	PRINT 'Membro - Antigo'
	SELECT * FROM SituacaoMembro WHERE MembroId = @IdAntigo
END