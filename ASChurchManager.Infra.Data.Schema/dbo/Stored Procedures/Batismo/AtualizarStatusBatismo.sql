CREATE PROCEDURE [dbo].[AtualizarStatusBatismo]
	@Id INT,
	@Status INT
AS
BEGIN
	UPDATE
		Batismo
	SET
		Status = @Status
		, DataAlteracao = GETDATE()
	WHERE
		Id = @Id
END
