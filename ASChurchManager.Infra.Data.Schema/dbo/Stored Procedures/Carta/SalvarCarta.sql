CREATE PROCEDURE SalvarCarta
	@Id BIGINT
	, @MembroId INT
	, @TipoCarta TINYINT
	, @CongregacaoOrigemId INT
	, @CongregacaoDestID INT
	, @CongregacaoDest VARCHAR(100)
	, @Observacao VARCHAR(500)
	, @DataValidade DATETIMEOFFSET
	, @StatusCarta INT
	, @CodigoRecebimento VARCHAR(10)
	, @TemplateId BIGINT
	, @UsuarioID INT
AS
BEGIN	
	IF (ISNULL(@Id,0) > 0)
	BEGIN
		UPDATE dbo.Carta
		SET
			MembroId = @MembroId
			, TipoCarta = @TipoCarta
			, CongregacaoOrigemId = @CongregacaoOrigemId
			, CongregacaoDestID = CASE WHEN @TipoCarta IN (2, 3) THEN 0 ELSE @CongregacaoDestID END
			, CongregacaoDest =  @CongregacaoDest 
			, Observacao = @Observacao
			, DataValidade = @DataValidade
			, DataAlteracao = GETDATE()
			, StatusCarta = @StatusCarta
			, TemplateId = @TemplateId
		WHERE 
			Id = @Id
	END
	ELSE
	BEGIN
		INSERT INTO dbo.Carta(
			MembroId
			, TipoCarta
			, CongregacaoOrigemId
			, CongregacaoDestId
			, CongregacaoDest
			, Observacao
			, StatusCarta
			, DataValidade
			, DataAlteracao
			, DataCriacao
			, CodigoRecebimento
			, TemplateId
			, IdCadastro
		)
		VALUES(		
			@MembroId
			, @TipoCarta
			, @CongregacaoOrigemId
			, CASE WHEN @TipoCarta IN (2, 3) THEN 0 
			  ELSE @CongregacaoDestID END
			, @CongregacaoDest
			, @Observacao
			, 1 -- Aguardando aprovação
			, @DataValidade
			, GETDATE()
			, GETDATE()
			, @CodigoRecebimento
			, @TemplateId
			, @UsuarioID
		)
		SELECT @Id = SCOPE_IDENTITY()
		
		IF (@TipoCarta IN (2, 3)) -- Mudança/Recomendacao
		BEGIN
			EXEC AprovaCarta @Id, @UsuarioID
		END
	END		

	SELECT @Id
	RETURN @Id
END