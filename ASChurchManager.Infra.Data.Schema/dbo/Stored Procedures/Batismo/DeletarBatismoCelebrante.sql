CREATE PROCEDURE dbo.DeletarBatismoCelebrante
	@Id INT
AS
BEGIN
DELETE
	FROM dbo.BatismoCelebrante
	WHERE
		BatismoId = @Id
END
