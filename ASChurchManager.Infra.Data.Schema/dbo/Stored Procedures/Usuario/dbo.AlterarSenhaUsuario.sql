CREATE PROCEDURE dbo.AlterarSenhaUsuario
	@Id INT 
	, @Senha VARCHAR(1000)
	, @UsuarioAlteracao INT
AS
BEGIN
	
	SET NOCOUNT ON
	
	UPDATE U
	SET	
		Senha = @senha
		, AlterarSenhaProxLogin = CASE WHEN @UsuarioAlteracao = @Id THEN 0 ELSE 1 END
		, DataAlteracao = GETDATE()
	FROM dbo.Usuario U
	WHERE 
		Id = @Id
	
	SELECT @Id
	RETURN @Id

END