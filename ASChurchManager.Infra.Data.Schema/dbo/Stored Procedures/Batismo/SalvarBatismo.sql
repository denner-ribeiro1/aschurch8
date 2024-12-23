CREATE PROCEDURE [dbo].[SalvarBatismo]
	@Id INT, 
    @DataMaximaCadastro DATETIMEOFFSET, 
    @DataBatismo DATETIMEOFFSET,
	@Status INT,
	@IdadeMinima INT
AS
BEGIN
	IF (@Id > 0)
	BEGIN
		UPDATE 
			Batismo
		SET
			DataBatismo = @DataBatismo
			, DataMaximaCadastro = @DataMaximaCadastro
			, DataAlteracao = GETDATE()
			, Status = @Status
			, IdadeMinima = @IdadeMinima
		WHERE
			Id = @Id
		
		/*ATUALIZO A DATA PREVISTA PARA TODOS OS MEMBROS Q JA ESTAVAM CADASTRADOS PARA O BATISMO*/
		UPDATE
			Membro
		SET
			DataPrevistaBatismo = @DataBatismo
		WHERE
			BatismoId = @Id
	END
	ELSE
	BEGIN
		SELECT @Id = ISNULL(MAX(Id), 0) + 1 FROM Batismo

		INSERT INTO Batismo(
			Id
			, DataMaximaCadastro
			, DataBatismo
			, DataCriacao
			, DataAlteracao
			, Status
			, IdadeMinima
		)
		SELECT
			@Id 
			, @DataMaximaCadastro
			, @DataBatismo
			, GETDATE()
			, GETDATE()
			, @Status
			, @IdadeMinima
	END
	--Excluo os celebrantes para serem incluidos novamente.
	DELETE BatismoCelebrante WHERE BatismoId = @Id

	SELECT @Id
	RETURN @Id
END
