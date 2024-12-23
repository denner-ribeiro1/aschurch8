CREATE PROCEDURE dbo.DeletarNascimento
	@Id INT
AS
BEGIN
	DELETE dbo.Nascimento
	WHERE Id = @Id
END