CREATE PROC [dbo].[SalvarCongregacao]
	@Id INT 
	, @Nome VARCHAR(50)
	, @Logradouro VARCHAR(100)
	, @Numero VARCHAR(10)
	, @Complemento VARCHAR(100)
	, @Bairro VARCHAR(100)
	, @Cidade VARCHAR(100)
	, @Estado VARCHAR(30)
	, @Pais VARCHAR(100)
	, @Cep VARCHAR(15)
	, @CongregacaoResponsavelId INT
	, @PastorResponsavelId INT
	, @CNPJ varchar(18)
AS
BEGIN

	DECLARE @CongregacaoID INT = ISNULL(@Id, 0)

	IF(ISNULL(@CongregacaoResponsavelId,0) = 0)
	BEGIN
		SELECT 
			@CongregacaoResponsavelId = Id 
		FROM 
			dbo.Congregacao
		WHERE 
			Sede = 1
	END

	IF (@Id > 0 )
	BEGIN 
		UPDATE dbo.Congregacao
		SET 
			Nome = @Nome
			, Logradouro = @Logradouro
			, Numero = @Numero
			, Complemento = @Complemento
			, Bairro = @Bairro
			, Cidade = @Cidade
			, Estado = @Estado
			, Cep = @Cep
			, Pais = @Pais
			, DataAlteracao = GETDATE()
			, CongregacaoResponsavelId = @CongregacaoResponsavelId
			, PastorResponsavelId = @PastorResponsavelId
			, CNPJ = @CNPJ
		WHERE Id = @Id

	END
	ELSE
	BEGIN 

		INSERT INTO dbo.Congregacao
		(
			Nome
		   , Logradouro
		   , Numero
		   , Complemento
		   , Bairro
		   , Cidade
		   , Estado
		   , Pais
		   , Cep
		   , DataCriacao
		   , DataAlteracao
		   , CongregacaoResponsavelId
		   , PastorResponsavelId
		   , CNPJ
		)
		SELECT
			@Nome
			, @Logradouro 
			, @Numero 
			, @Complemento 
			, @Bairro 
			, @Cidade 
			, @Estado 
			, @Pais
			, @Cep 
			, GETDATE() 
			, GETDATE()
			, @CongregacaoResponsavelId
			, @PastorResponsavelId
			, @CNPJ

		SELECT @CongregacaoID = SCOPE_IDENTITY()

	END

	SELECT @CongregacaoID
	RETURN @CongregacaoID
END