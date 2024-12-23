CREATE PROCEDURE [dbo].[RelBatismo]
	@BatismoId INT,
	@DataBatismo DATETIME,
	@UsuarioID BIGINT
AS
BEGIN
	SELECT 
		B.Id,
		B.DataBatismo,
		B.Status
	FROM Batismo B
	WHERE 
		(ISNULL(@BatismoId, 0) = 0 OR B.Id = @BatismoId)
		AND (ISNULL(@DataBatismo, 0) = 0 OR CONVERT(DATE, B.DataBatismo) = CONVERT(DATE, @DataBatismo))
	ORDER BY
		B.DataBatismo
END
