CREATE PROCEDURE AtualizarMembroFotoUrl
	@Id INT,
	@FotoUrl VARCHAR(500)
AS
BEGIN
	UPDATE
		Membro	
	SET
		FotoUrl = @FotoUrl
	WHERE
		Id = @Id
END