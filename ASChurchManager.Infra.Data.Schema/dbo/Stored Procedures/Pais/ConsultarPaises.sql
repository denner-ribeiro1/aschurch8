CREATE PROCEDURE [dbo].[ConsultarPaises]
AS
BEGIN
	SELECT Id, Nome, Abrev FROM Pais
END