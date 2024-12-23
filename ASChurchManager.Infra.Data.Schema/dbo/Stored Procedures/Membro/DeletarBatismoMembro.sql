CREATE PROCEDURE DeletarBatismoCandidato
	@MembroId INT
AS
BEGIN
	DELETE
		BatismoCandidato
	WHERE
		MembroId = @MembroId
		AND Situacao = 0
END