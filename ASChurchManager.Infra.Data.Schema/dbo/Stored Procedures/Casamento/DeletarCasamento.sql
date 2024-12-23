CREATE PROCEDURE dbo.DeletarCasamento
	@Id INT
AS
BEGIN
	DELETE 
		dbo.Casamento
	WHERE
		Id = @Id
END
