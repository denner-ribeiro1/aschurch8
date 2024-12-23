CREATE  PROCEDURE RelBatismoPastorCelebrante
@BatismoId INT
	AS
BEGIN
	select M.Nome from BatismoCelebrante B 
	INNER JOIN Membro M
	ON B.MembroId = M.Id
	INNER JOIN Batismo T
	ON T.Id = B.BatismoId
	WHERE B.BatismoId = @BatismoId
END