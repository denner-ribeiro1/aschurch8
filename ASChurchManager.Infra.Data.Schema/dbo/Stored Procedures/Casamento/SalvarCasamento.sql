CREATE PROC SalvarCasamento
	@Id INT,
	@CongregacaoId INT,
	@PastorId INT,
	@PastorNome VARCHAR(100),
	@DataHoraInicio DATETIMEOFFSET,
	@DataHoraFinal DATETIMEOFFSET,
	@NoivoId INT,
	@NoivoNome VARCHAR(100),
	@PaiNoivoId INT,
	@PaiNoivoNome VARCHAR(100),
	@MaeNoivoId INT,
	@MaeNoivoNome VARCHAR(100),
	@NoivaId INT,
	@NoivaNome VARCHAR(100),
	@PaiNoivaId INT,
	@PaiNoivaNome VARCHAR(100),
	@MaeNoivaId INT,
	@MaeNoivaNome VARCHAR(100)
AS
BEGIN
	DECLARE @CasamentoID INT = isnull(@Id,0)

    IF (@Id > 0 )
    BEGIN 
        UPDATE 
            dbo.Casamento
        SET
			CongregacaoId = @CongregacaoId,
			PastorId = @PastorId,
			PastorNome = @PastorNome,
			DataHoraInicio = @DataHoraInicio,
			DataHoraFinal = @DataHoraFinal,
			NoivoId = CASE WHEN ISNULL(@NoivoId, 0) > 0 THEN @NoivoId ELSE NULL END,
			NoivoNome = @NoivoNome,
			PaiNoivoId = CASE WHEN ISNULL(@PaiNoivoId, 0) > 0 THEN @PaiNoivoId ELSE NULL END,
			PaiNoivoNome = @PaiNoivoNome,
			MaeNoivoId = CASE WHEN ISNULL(@MaeNoivoId, 0) > 0 THEN @MaeNoivoId ELSE NULL END,
			MaeNoivoNome = @MaeNoivoNome,
			NoivaId = CASE WHEN ISNULL(@NoivaId, 0) > 0 THEN @NoivaId ELSE NULL END,
			NoivaNome = @NoivaNome,
			PaiNoivaId = CASE WHEN ISNULL(@PaiNoivaId, 0) > 0 THEN @PaiNoivaId ELSE NULL END,
			PaiNoivaNome = @PaiNoivaNome,
			MaeNoivaId = CASE WHEN ISNULL(@MaeNoivaId, 0) > 0 THEN @MaeNoivaId ELSE NULL END,
			MaeNoivaNome = @MaeNoivaNome,
			DataAlteracao = GETDATE()
        WHERE 
            Id = @Id
    END
    ELSE
    BEGIN 
        INSERT INTO dbo.Casamento (
			CongregacaoId,
			PastorId,
			PastorNome,
			DataHoraInicio,
			DataHoraFinal,
			NoivoId,
			NoivoNome,
			PaiNoivoId,
			PaiNoivoNome,
			MaeNoivoId,
			MaeNoivoNome,
			NoivaId,
			NoivaNome,
			PaiNoivaId,
			PaiNoivaNome,
			MaeNoivaId,
			MaeNoivaNome,
			DataCriacao,
			DataAlteracao
		)
		VALUES (
			@CongregacaoId,
			@PastorId,
			@PastorNome,
			@DataHoraInicio,
			@DataHoraFinal,
			CASE WHEN ISNULL(@NoivoId, 0) > 0 THEN @NoivoId ELSE NULL END,
			@NoivoNome,
			CASE WHEN ISNULL(@PaiNoivoId, 0) > 0 THEN @PaiNoivoId ELSE NULL END,
			@PaiNoivoNome,
			CASE WHEN ISNULL(@MaeNoivoId, 0) > 0 THEN @MaeNoivoId ELSE NULL END,
			@MaeNoivoNome,
			CASE WHEN ISNULL(@NoivaId, 0) > 0 THEN @NoivaId ELSE NULL END,
			@NoivaNome,
			CASE WHEN ISNULL(@PaiNoivaId, 0) > 0 THEN @PaiNoivaId ELSE NULL END,
			@PaiNoivaNome,
			CASE WHEN ISNULL(@MaeNoivaId, 0) > 0 THEN @MaeNoivaId ELSE NULL END,
			@MaeNoivaNome,
			GETDATE(),
			GETDATE()
		)
		SELECT @CasamentoID = SCOPE_IDENTITY()
    END
	SELECT @CasamentoID
	RETURN @CasamentoID
END
