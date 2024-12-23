CREATE PROC ExcluirCodigoRegistroMembro
	@Id int
AS
BEGIN
	PRINT 'Membro'
	SELECT * FROM Membro WHERE Id = @Id
	PRINT 'CargoMembro'
	SELECT * FROM CargoMembro WHERE MembroId = @Id
	PRINT 'ObservacaoMembro'
	SELECT * FROM ObservacaoMembro WHERE MembroId = @Id
	PRINT 'SituacaoMembro'
	SELECT * FROM SituacaoMembro WHERE MembroId = @Id
	PRINT 'SituacaoMembro'
	SELECT * FROM HistoricoAprovacaoMembro WHERE MembroId = @Id

	BEGIN TRY 
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
			Membro 
		WHERE 
			Id = @Id 
		
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
		ROLLBACK TRAN
	END CATCH
END
GO