CREATE PROC SalvarUsuario
	@Id INT 
	, @Nome VARCHAR(100)
	, @Username VARCHAR(30)
	, @Senha VARCHAR(1000)
	, @Email VARCHAR(50)
	, @AlterarSenhaProxLogin CHAR(1)
	, @CongregacaoId INT 
	, @PerfilId INT 
	, @PermiteAprovarMembro BIT
	, @PermiteImprimirCarteirinha BIT
	, @PermiteExcluirSituacaoMembro BIT
	, @PermiteCadBatismoAposDataMaxima BIT
	, @PermiteExcluirMembros BIT
	, @PermiteExcluirCargoMembro BIT
	, @PermiteExcluirObservacaoMembro BIT
AS
BEGIN
	DECLARE @UsuarioID INT = ISNULL(@Id, 0)

	IF (@Id > 0 )
	BEGIN 
		UPDATE 
			dbo.Usuario
		SET 
			Nome = @Nome
			, Username = LOWER(@Username)
			, Email = LOWER(@Email)
			, DataAlteracao = GETDATE()
			, CongregacaoId = @CongregacaoId
			, PerfilId = @PerfilId
			, PermiteAprovarMembro = @PermiteAprovarMembro
			, PermiteImprimirCarteirinha = @PermiteImprimirCarteirinha
			, PermiteExcluirSituacaoMembro = @PermiteExcluirSituacaoMembro
			, PermiteCadBatismoAposDataMaxima = @PermiteCadBatismoAposDataMaxima
			, PermiteExcluirMembros = @PermiteExcluirMembros
			, PermiteExcluirCargoMembro = @PermiteExcluirCargoMembro
			, PermiteExcluirObservacaoMembro = @PermiteExcluirObservacaoMembro
		WHERE 
			Id = @Id
	END
	ELSE
	BEGIN 
		INSERT INTO dbo.Usuario
		(
			Nome
			, Username
			, Senha
			, Email
			, AlterarSenhaProxLogin
			, DataCriacao
			, DataAlteracao
			, CongregacaoId
			, Status
			, PerfilId
			, PermiteAprovarMembro
			, PermiteImprimirCarteirinha
			, PermiteExcluirSituacaoMembro
			, PermiteCadBatismoAposDataMaxima
			, PermiteExcluirMembros
			, PermiteExcluirCargoMembro
			, PermiteExcluirObservacaoMembro
		)
		Select
			@Nome
			, LOWER(@Username)
			, @Senha
			, LOWER(@Email)
			, @AlterarSenhaProxLogin
			, GETDATE()
			, GETDATE()
			, @CongregacaoId
			, 1
			, @PerfilId
			, @PermiteAprovarMembro
			, @PermiteImprimirCarteirinha
			, @PermiteExcluirSituacaoMembro
			, @PermiteCadBatismoAposDataMaxima
			, @PermiteExcluirMembros
			, @PermiteExcluirCargoMembro
			, @PermiteExcluirObservacaoMembro

		SELECT @UsuarioID = SCOPE_IDENTITY()
	END

	SELECT @UsuarioID
	RETURN @UsuarioID
END