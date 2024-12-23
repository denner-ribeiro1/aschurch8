CREATE PROCEDURE dbo.SalvarEvento
	@Id BIGINT
	, @CongregacaoId INT
	, @TipoEventoId INT
	, @Descricao VARCHAR(100)
	, @DataHoraInicio DATETIMEOFFSET
	, @DataHoraFim DATETIMEOFFSET
	, @Observacoes VARCHAR(500)
	, @Frequencia TINYINT
	, @Quantidade INT
	, @AlertarEventoMesmoDia BIT
AS
BEGIN
	SET NOCOUNT ON

	IF(ISNULL(@Id,0) > 0)
	BEGIN
		UPDATE dbo.Evento
		SET
			Descricao = @Descricao
			, CongregacaoId= @CongregacaoId
			, TipoEventoId = @TipoEventoId
			, DataHoraInicio = @DataHoraInicio
			, DataHoraFim = @DataHoraFim
			, Observacoes = @Observacoes
			, DataAlteracao = GETDATE()
			, AlertarEventoMesmoDia = @AlertarEventoMesmoDia
		WHERE Id = @Id

		DELETE dbo.Evento
		WHERE IdEventoOriginal = @Id
	END
	ELSE
	BEGIN
		INSERT INTO dbo.Evento
		(
			CongregacaoId
			, TipoEventoId
			, Descricao
			, DataHoraInicio
			, DataHoraFim
			, Observacoes
			, DataCriacao
			, Frequencia
			, Quantidade
			, AlertarEventoMesmoDia
		)
		SELECT 
			@CongregacaoId
			, @TipoEventoId
			, @Descricao
			, @DataHoraInicio
			, @DataHoraFim
			, @Observacoes
			, GETDATE()
			, @Frequencia
			, @Quantidade
			, @AlertarEventoMesmoDia

		SELECT @Id = SCOPE_IDENTITY()
		IF((@Frequencia BETWEEN 2 AND 5) AND @Quantidade > 1)
		BEGIN
			DECLARE @DataHoraInicioNova DATETIMEOFFSET, @DataHoraFimNova DATETIMEOFFSET, @SEQ INT = 1
			WHILE (@SEQ < @Quantidade)
			BEGIN
				IF (@Frequencia = 2) --DIARIO
				BEGIN
					SELECT
						@DataHoraInicioNova = DATEADD(DAY, @SEQ, @DataHoraInicio),
						@DataHoraFimNova = DATEADD(DAY, @SEQ, @DataHoraFim)
				END	
				ELSE IF (@Frequencia = 3) --SEMANAL
				BEGIN
					SELECT
						@DataHoraInicioNova = DATEADD(WEEK, @SEQ, @DataHoraInicio),
						@DataHoraFimNova = DATEADD(WEEK, @SEQ, @DataHoraFim)
				END	
				ELSE IF (@Frequencia = 4) --MENSAL
				BEGIN
					SELECT
						@DataHoraInicioNova = DATEADD(MONTH, @SEQ, @DataHoraInicio),
						@DataHoraFimNova = DATEADD(MONTH, @SEQ, @DataHoraFim)
				END	
				ELSE IF (@Frequencia = 5) --ANUAL
				BEGIN
					SELECT
						@DataHoraInicioNova = DATEADD(YEAR, @SEQ, @DataHoraInicio),
						@DataHoraFimNova = DATEADD(YEAR, @SEQ, @DataHoraFim)
				END	

				INSERT INTO dbo.Evento
				(
					IdEventoOriginal
					, CongregacaoId
					, TipoEventoId
					, Descricao
					, DataHoraInicio
					, DataHoraFim
					, Observacoes
					, DataCriacao
					, Frequencia
					, Quantidade
					, AlertarEventoMesmoDia
				)
				SELECT 
					@Id
					, @CongregacaoId
					, @TipoEventoId
					, @Descricao
					, @DataHoraInicioNova
					, @DataHoraFimNova
					, @Observacoes
					, GETDATE()
					, @Frequencia
					, @Quantidade
					, @AlertarEventoMesmoDia
				
				SELECT @SEQ = @SEQ + 1	
			END
		END
	END	

	SELECT @Id
	RETURN @Id	
	
END