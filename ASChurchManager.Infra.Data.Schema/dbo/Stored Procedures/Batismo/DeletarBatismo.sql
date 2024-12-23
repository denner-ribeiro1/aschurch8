CREATE PROCEDURE dbo.DeletarBatismo
	@Id INT
AS
BEGIN
	UPDATE 
		dbo.Membro
	SET 
		BatismoId = NULL
		, Status = 1
		, DataBatismoAguas = NULL
		, DataPrevistaBatismo = NULL
		, TipoMembro = 1 /*Congregado*/
		, DataAlteracao = GETDATE()
		, BatismoSituacao = 0
	WHERE 
		BatismoId = @Id

	DELETE 
		BatismoCandidato
    WHERE 
		BatismoId = @Id

	DELETE
		dbo.Batismo
	WHERE
		Id = @Id
END