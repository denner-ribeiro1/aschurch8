CREATE PROCEDURE [dbo].[AlterarSkinUsuario]
	@Skin VARCHAR(30),
	@UsuarioID int
AS
BEGIN
	UPDATE 
		Usuario
	SET
		Skin = @Skin
	WHERE
		Id = @UsuarioID
END