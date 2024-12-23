CREATE PROCEDURE [dbo].[ListarDatasBatismo]
AS
BEGIN
	SELECT 
		*
	FROM
		Batismo B
	ORDER BY 
		DataBatismo	DESC
END