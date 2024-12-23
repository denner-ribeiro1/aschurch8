CREATE  PROCEDURE dbo.SalvarTemplate
	@Id BIGINT
	, @Nome VARCHAR(100)
	, @Conteudo VARCHAR(8000) 
	, @Tipo TINYINT
	, @MargemAbaixo INT
	, @MargemAcima INT
	, @MargemDireita INT
	, @MargemEsquerda INT
AS
BEGIN
	SET NOCOUNT ON

	IF(ISNULL(@Id, 0) > 0)
	BEGIN
		UPDATE 
			dbo.Template
		SET
			Nome = @Nome
			, Conteudo = @Conteudo
			, Tipo = @Tipo
			, DataAlteracao = GETDATE()
			, MargemAbaixo = @MargemAbaixo 
			, MargemAcima = @MargemAcima 
			, MargemDireita = @MargemDireita
			, MargemEsquerda = @MargemEsquerda
		WHERE
			Id = @Id
	END
	ELSE
	BEGIN
		INSERT INTO dbo.Template(Nome, Conteudo, Tipo, Status, DataCriacao, DataAlteracao, MargemAbaixo, MargemAcima, MargemDireita, MargemEsquerda)
		SELECT @Nome, @Conteudo, @Tipo, 1, GETDATE(), GETDATE(), @MargemAbaixo, @MargemAcima, @MargemDireita, @MargemEsquerda

		SELECT @Id = SCOPE_IDENTITY()
	END

	SELECT @Id
	RETURN @Id
END