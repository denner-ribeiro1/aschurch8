
CREATE PROCEDURE dbo.DeletarCargo
	@Id INT
AS
BEGIN
	DELETE dbo.Cargo
	WHERE Id = @Id
END