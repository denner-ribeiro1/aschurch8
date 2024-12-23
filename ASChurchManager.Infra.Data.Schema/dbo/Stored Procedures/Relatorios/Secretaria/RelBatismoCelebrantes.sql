CREATE PROCEDURE [dbo].[RelBatismoCelebrantes]
	@BatismoId INT
AS
BEGIN
	SELECT 
		B.BatismoId,
		M.Id,
		M.Nome
	FROM BatismoCelebrante B
	INNER JOIN Membro M ON M.Id = B.MembroId
	WHERE 
		B.BatismoId = @BatismoId
END