CREATE PROCEDURE [dbo].[DeletePresencaInscricao]
	@Id INT
AS
BEGIN
	DELETE
		PresencaInscricao 
	WHERE
		Id =  @Id
END
