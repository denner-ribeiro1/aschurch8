

CREATE PROCEDURE dbo.DeletarTemplate
	@Id BIGINT 
AS
BEGIN

	DELETE
	FROM dbo.Template
	WHERE Id = @Id

END