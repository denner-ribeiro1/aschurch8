CREATE PROCEDURE dbo.DeletarGrupo
	@Id INT
AS
BEGIN
	DELETE dbo.Grupo
	WHERE Id = @Id
END