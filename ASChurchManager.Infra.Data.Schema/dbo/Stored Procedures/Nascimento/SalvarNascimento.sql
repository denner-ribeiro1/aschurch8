CREATE PROC SalvarNascimento
    @Id INT,
	@CongregacaoId INT,
	@IdMembroPai INT,
	@IdMembroMae INT,
	@Pai VARCHAR(100),
	@Mae VARCHAR(100),
	@Crianca VARCHAR(100),
	@Sexo CHAR(1),
	@DataApresentacao DATETIMEOFFSET,
	@DataNascimento DATETIMEOFFSET,
	@Pastor VARCHAR(100),
	@PastorId INT
AS
BEGIN
	DECLARE @NascimentoID INT = isnull(@Id,0)

    IF (@Id > 0 )
    BEGIN 
        UPDATE 
            dbo.Nascimento
        SET 
			CongregacaoId = @CongregacaoId,
			NomePai = @Pai,
			NomeMae = @Mae,
			IdMembroPai = @IdMembroPai,
			IdMembroMae = @IdMembroMae,
			Crianca = @Crianca,
			Sexo = @Sexo,
			Pastor = @Pastor,
			PastorId = @PastorId,
			DataApresentacao = @DataApresentacao,
			DataNascimento = @DataNascimento,
			DataAlteracao = GETDATE()
        WHERE 
            Id = @Id
    END
    ELSE
    BEGIN 
		INSERT INTO dbo.Nascimento(
			CongregacaoId,
			NomePai,
			NomeMae,
			IdMembroPai,
			IdMembroMae,
			Crianca,
			Sexo,
			Pastor,
			PastorId,
			DataApresentacao,
			DataNascimento,
			DataAlteracao,
			DataCriacao)
		SELECT
			@CongregacaoId,
			@Pai,
			@Mae,
			@IdMembroPai,
			@IdMembroMae,
			@Crianca,
			@Sexo,
			@Pastor,
			@PastorId,
			@DataApresentacao,
			@DataNascimento,
			GETDATE(),
			GETDATE()

		SELECT @NascimentoID = SCOPE_IDENTITY()
    END

	SELECT @NascimentoID
	RETURN @NascimentoID
END
