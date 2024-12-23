CREATE PROCEDURE IncluirCandidatoBatismo
	@BatismoId INT, 
    @MembroId INT
AS
BEGIN
	INSERT INTO BatismoCandidato
           (BatismoId
           ,MembroId
           ,Situacao)
     VALUES
           (
            @BatismoId
           ,@MembroId
           ,0)
END