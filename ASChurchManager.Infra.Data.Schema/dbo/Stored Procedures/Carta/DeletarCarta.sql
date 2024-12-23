create PROCEDURE dbo.DeletarCarta
	@Id INT
AS
BEGIN
	UPDATE
		dbo.Carta
	SET
		StatusCarta = 3
	WHERE
		Id = @Id
END