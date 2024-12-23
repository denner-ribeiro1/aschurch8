CREATE PROCEDURE dbo.DeletarMembro
	@Id INT,
	@Trans Char(1) = 'S'
AS
BEGIN
	BEGIN TRY 
		IF (ISNULL(@Trans, 'S') = 'S')
			BEGIN TRAN

		DELETE
			CargoMembro
		WHERE
			MembroId = @Id

		DELETE
			ObservacaoMembro
		WHERE
			MembroId = @Id

		DELETE
			SituacaoMembro
		WHERE
			MembroId = @Id
		
		DELETE
			HistoricoAprovacaoMembro
		WHERE
			MembroId = @Id
		
		DELETE
			Carta
		WHERE
			MembroId = @Id

		DELETE 
			CursoArquivoMembro
		WHERE 
			MembroId = @Id

		DELETE 
			BatismoCandidato
		WHERE 
			MembroId = @Id
			
		DELETE 
			Membro 
		WHERE 
			Id = @Id 
		IF (ISNULL(@Trans, 'S') = 'S')
			COMMIT
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE(),
			   @ErrorSeverity = ERROR_SEVERITY(),
			   @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
		IF (ISNULL(@Trans, 'S') = 'S')
			ROLLBACK TRAN
	END CATCH
END