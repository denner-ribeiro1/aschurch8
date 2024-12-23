CREATE PROCEDURE RelBatismoPastorPresidente
	AS
BEGIN
	select M.Nome from Congregacao C
	INNER JOIN Membro M ON M.Id = C.PastorResponsavelId
	where C.id  = 1
END