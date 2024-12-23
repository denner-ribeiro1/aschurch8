
CREATE PROCEDURE dbo.SalvarPerfilRotina
	@IdPerfil INT
	, @IdRotina INT
AS
BEGIN

	INSERT INTO dbo.PerfilRotina(IdPerfil, IdRotina)
	SELECT @IdPerfil, @IdRotina

END