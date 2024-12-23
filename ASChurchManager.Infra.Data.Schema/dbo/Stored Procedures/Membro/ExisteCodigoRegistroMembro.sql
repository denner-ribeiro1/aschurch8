CREATE PROCEDURE ExisteCodigoRegistroMembro
	@Id VARCHAR(10)
AS
BEGIN

	SELECT 
		TOP 1 1 
	FROM 
		dbo.Membro
	WHERE Id = @Id

END