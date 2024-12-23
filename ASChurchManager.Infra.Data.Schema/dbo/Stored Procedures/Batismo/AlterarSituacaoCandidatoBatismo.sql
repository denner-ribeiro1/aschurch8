CREATE PROCEDURE AlterarSituacaoCandidatoBatismo
	@BatismoId INT, 
    @MembroId INT,
	@Situacao TINYINT
AS
BEGIN
	UPDATE BatismoCandidato
	   SET 
		  Situacao = @Situacao
		  ,DataConfirmacao = GETDATE()
	 WHERE 
		BatismoId = @BatismoId AND
		MembroId = @MembroId
END
