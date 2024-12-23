CREATE PROCEDURE ExcluirCandidatoBatismo
	@BatismoId INT, 
	@MembroId INT
AS
BEGIN
	DELETE FROM BatismoCandidato
      WHERE BatismoId = @BatismoId AND 
			MembroId = @MembroId
END
