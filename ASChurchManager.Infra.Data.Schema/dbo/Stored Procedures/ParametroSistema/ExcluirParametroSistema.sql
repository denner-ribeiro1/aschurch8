CREATE PROCEDURE ExcluirParametroSistema
	@Nome VARCHAR(60)
	, @Valor VARCHAR(100)
AS
BEGIN

	DELETE FROM dbo.ParametroSistema
	WHERE 
		Nome = @Nome
		AND Valor = @Valor

END