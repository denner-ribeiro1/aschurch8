CREATE PROCEDURE [dbo].[SelecionaUltimaDataBatismo]
AS
BEGIN
	SELECT 
		*
	FROM
		Batismo B
	WHERE
		B.Id = (SELECT MAX(X.Id) FROM Batismo X)
END