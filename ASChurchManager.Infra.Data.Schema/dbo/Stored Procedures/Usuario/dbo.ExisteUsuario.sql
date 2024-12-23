CREATE PROCEDURE dbo.ExisteUsuario
	@Username VARCHAR(50) 
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT 
		TOP 1 1 
	FROM 
		dbo.Usuario
	WHERE 
		Username = @Username
END