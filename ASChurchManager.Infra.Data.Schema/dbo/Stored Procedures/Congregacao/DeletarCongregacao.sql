CREATE PROCEDURE dbo.DeletarCongregacao
	@Id INT,
	@CongregacaoDestId INT = 0,
	@UsuarioId INT = 0
AS
BEGIN
	BEGIN TRY 
		BEGIN TRAN

		DECLARE @CongrOrig VARCHAR(50), @CongrDest VARCHAR(50)
		SELECT @CongrOrig = Nome FROM Congregacao WHERE Id = @Id
		DECLARE @DATA SMALLDATETIME = GETDATE()

		IF ISNULL(@CongregacaoDestId, 0) > 0
		BEGIN
			SELECT @CongrDest = Nome FROM Congregacao WHERE Id = @CongregacaoDestId

			/*INCLUINDO OBSERVACAO NO MEMBRO*/
			INSERT INTO ObservacaoMembro(MembroId, Observacao, UsuarioId, DataCadastro)
			SELECT Id, 'Transferido por exclusão da Congregação ' + @CongrOrig + ' para ' + @CongrDest, @UsuarioId, @DATA FROM Membro WHERE CongregacaoId = @Id

			/*ALTERANDO AS CONGREGAÇÕES DE TODOS O MEMBROS PARA A NOVA CONGREGAÇÃO*/
			UPDATE 
				dbo.Membro 
			SET 
				CongregacaoId = @CongregacaoDestId
				, DataAlteracao = @DATA
			WHERE
				CongregacaoId = @Id	

			/*ALTERANDO AS CONGREGAÇÕES DE TODOS O NASCIMENTOS PARA A NOVA CONGREGAÇÃO*/
			UPDATE 
				dbo.Nascimento 
			SET 
				CongregacaoId = @CongregacaoDestId
				, DataAlteracao = @DATA	
			WHERE
				CongregacaoId = @Id	

			/*ALTERANDO AS CONGREGAÇÕES DE TODOS OS CASAMENTO PARA A NOVA CONGREGAÇÃO*/
			UPDATE 
				dbo.Casamento
			SET 
				CongregacaoId = @CongregacaoDestId
				, DataAlteracao = @DATA	
			WHERE
				CongregacaoId = @Id	

			/*ALTERANDO AS CONGREGAÇÕES DE TODOS OS CURSOS/EVENTOS PARA A NOVA CONGREGAÇÃO*/

			UPDATE
				Presenca
			SET 
				CongregacaoId = @CongregacaoDestId
			WHERE
				CongregacaoId = @Id

			UPDATE
				PresencaInscricao
			SET 
				CongregacaoId = @CongregacaoDestId
			WHERE
				CongregacaoId = @Id

			/*ALTERANDO AS CONGREGAÇÕES DE TODOS OS USUARIOS PARA A NOVA CONGREGAÇÃO*/
			UPDATE
				Usuario
			SET 
				CongregacaoId = @CongregacaoDestId
			WHERE
				CongregacaoId = @Id

		END

		/*ATUALIZANDO OS DADOS DA CONGREGAÇÃO*/
		UPDATE 
			Congregacao  
		SET 
			Situacao = 0,
			Nome = LEFT('CONGREGAÇÃO EXCLUÍDA - ' + @CongrOrig, 50),
			CongregacaoResponsavelId = 0,
			PastorResponsavelId = 0,
			DataAlteracao = @DATA
		WHERE Id = @Id

		DELETE Evento WHERE CongregacaoId = @Id
		EXEC DeletarCongregacaoGrupo @Id
		EXEC DeletarCongregacaoObreiro @Id
		EXEC DeletarCongregacaoObservacao @Id
		
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