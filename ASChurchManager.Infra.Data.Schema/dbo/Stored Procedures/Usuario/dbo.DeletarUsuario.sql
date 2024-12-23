CREATE PROCEDURE DeletarUsuario
	@Id INT
AS
BEGIN
	UPDATE 
		dbo.Usuario
	SET 
		Status = 2 -- Inativo
	WHERE 
		Id = @Id
END