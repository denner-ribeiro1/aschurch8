CREATE PROCEDURE dbo.IncluirAuditoria
    @UsuarioId BIGINT, 
	@Controle VARCHAR(50),
    @Acao VARCHAR(50),
	@Ip VARCHAR(50), 
    @Url VARCHAR(100),
	@Parametros VARCHAR(MAX),
	@Navegador VARCHAR(200)
AS
BEGIN
	INSERT INTO Auditoria
	(
		UsuarioId 
		, Controle
		, Acao
		, Ip
		, Url 
		, DataHora
		, Parametros
		, Navegador
	)
	SELECT
		@UsuarioId
		, @Controle
		, @Acao
		, @Ip
		, @Url 
		, GETDATE()
		, @Parametros
		, @Navegador
END
