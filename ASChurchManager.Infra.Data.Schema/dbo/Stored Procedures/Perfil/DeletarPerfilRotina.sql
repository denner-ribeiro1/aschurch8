CREATE PROCEDURE dbo.DeletarPerfilRotina
	@Id INT
AS
BEGIN
	BEGIN TRY 
		BEGIN TRAN
		DELETE dbo.PerfilRotina
		WHERE IdPerfil = @Id
		DELETE dbo.Perfil
		WHERE Id = @Id
		COMMIT TRAN
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