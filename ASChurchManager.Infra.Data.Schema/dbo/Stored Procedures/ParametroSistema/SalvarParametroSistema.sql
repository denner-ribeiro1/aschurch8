CREATE PROCEDURE SalvarParametroSistema
	@Nome VARCHAR(60)
	, @Descricao VARCHAR(100)
	, @Valor VARCHAR(100) = NULL
	, @SequencialNumerico BIT = 0
	, @Tabela VARCHAR(100) = NULL
	, @Colunas VARCHAR(255) = NULL
	, @SqlWhere VARCHAR(255) = NULL
AS
BEGIN

	DECLARE 
		@Id INT
		, @Sequencial INT
		, @Erro BIT
	
	SET @Erro = 0
	
	/*******************************************************************
		TESTE
	********************************************************************/
	--DECLARE
	--	@Nome VARCHAR(60)
	--	, @Descricao VARCHAR(100)
	--	, @Valor VARCHAR(100) 
	--	, @SequencialNumerico BIT
	--	, @Tabela VARCHAR(100) 
	--	, @Colunas VARCHAR(255) 
	--	, @SqlWhere VARCHAR(255) 
		
	--SELECT		
	--	@Nome = 'Religiao'
	--	, @Descricao = 'Católico'
	--	, @Valor = NULL
	--	, @SequencialNumerico = 1
	--	, @Tabela = NULL
	--	, @Colunas = NULL
	--	, @SqlWhere = NULL
	/*******************************************************************
		TESTE
	********************************************************************/
	
	IF EXISTS
	(
		SELECT TOP 1 1 FROM dbo.ParametroSistema
		WHERE Nome = @Nome
		AND Valor = @Valor
		AND Descricao <> @Descricao
	)
	BEGIN
	
		UPDATE P
		SET 
			Descricao = @Descricao
		FROM dbo.ParametroSistema P
		WHERE 
			Nome = @Nome
			AND Valor = @Valor
	
	END
	ELSE
	BEGIN
	
		IF NOT EXISTS
		(
			SELECT TOP 1 1 
			FROM dbo.ParametroSistema 
			WHERE 
				Nome = @Nome 
				AND Descricao = @Descricao
		)
		BEGIN
		
			BEGIN TRAN
			
				IF(@Valor = '')
				BEGIN
					SET @Valor = NULL
				END
			
				IF(@SequencialNumerico = 1 OR @Valor = NULL)
				BEGIN
					SELECT @Sequencial = ISNULL(MAX(CONVERT(INT, Valor)),0) + 1
					FROM dbo.ParametroSistema 
					WHERE Nome = @Nome
				END
				ELSE IF(@Valor IS NULL)
				BEGIN
					RAISERROR('Parâmetros inválidos - Valor não pode ficar vazio quando não é sequêncial numérico', 16, 1)
					SET @Erro = 1
				END
				
				IF(@Erro = 0)
				BEGIN
				
					INSERT INTO dbo.ParametroSistema 
					(
						Nome
						, Descricao
						, Valor
						, Tabela
						, Colunas
						, SqlWhere
						, DataCriacao
						, DataUltimaAlteracao
					)
					SELECT 
						@Nome
						, @Descricao
						, ISNULL(@Valor, @Sequencial)
						, @Tabela
						, @Colunas
						, @SqlWhere
						, GETDATE()
						, GETDATE()
					
				END
				
				
			IF(@@ERROR <> 0 AND @Erro <> 0)
			BEGIN
				ROLLBACK
				SELECT @Id = 0
			END
			ELSE
			BEGIN
				COMMIT
				SELECT @Id = SCOPE_IDENTITY()
			END
		
		END
		ELSE
		BEGIN
			RAISERROR('Parâmetro já existe', 16, 1)
		END
	END
	
	SELECT @Id
	RETURN @Id
END