CREATE PROCEDURE [dbo].[ListarPastorCelebrante]
@BatismoId int
AS
BEGIN
	SELECT M.Id, M.Nome FROM Membro M  INNER JOIN BatismoCelebrante B
	ON B.MembroId = M.Id
	WHERE B.BatismoId = @BatismoId
END
