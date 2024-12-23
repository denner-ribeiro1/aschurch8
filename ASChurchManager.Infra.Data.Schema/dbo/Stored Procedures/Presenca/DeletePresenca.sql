CREATE PROCEDURE [dbo].[DeletePresenca]
	@Id INT
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
	
		DELETE PresencaInscricaoArquivo
		WHERE Id IN (SELECT ArquivoId FROM PresencaInscricao WHERE PresencaId = @Id)

		DELETE PresencaInscricaoDatas
		WHERE InscricaoId IN (SELECT Id FROM PresencaInscricao WHERE PresencaId = @Id) 

		DELETE PresencaInscricao 
		WHERE PresencaId = @Id

		IF OBJECT_ID('TEMPDB..#EVENTOS') IS NOT NULL 
			DROP TABLE #EVENTOS
		SELECT EventoId INTO #EVENTOS FROM PresencaDatas WHERE PresencaId = @Id
	
		DELETE PresencaDatas WHERE PresencaId = @Id

		DELETE Evento
		WHERE Id IN (SELECT EventoId FROM #EVENTOS)

		DELETE Presenca WHERE Id = @Id
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