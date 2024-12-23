CREATE PROCEDURE dbo.ConsultarTemplatePorId
	@Id BIGINT 
	, @UsuarioId BIGINT
AS
BEGIN
	SELECT
		Id
		, Nome
		, Conteudo
		, Tipo
		, Status
		, DataCriacao
		, DataAlteracao
		, MargemAbaixo
		, MargemAcima
		, MargemDireita
		, MargemEsquerda
	FROM 
		dbo.Template
	WHERE 
		Id = @Id
END