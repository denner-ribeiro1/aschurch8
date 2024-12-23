CREATE PROCEDURE [dbo].[CancelarCarta]
	@IdCarta int,
	@UsuarioID int
AS
BEGIN
	BEGIN TRY 
		BEGIN TRAN
		DECLARE 
			@TipoCarta tinyint,
			@MembroId int,
			@StatusCarta tinyint,
			@CongregacaoOrigemId int,
			@CongregacaoDestId int,
			@UsuarioNome Varchar(100),
			@Obs Varchar(300)

		Select 
			@TipoCarta = TipoCarta,
			@MembroId = MembroId,
			@StatusCarta = StatusCarta,
			@CongregacaoOrigemId = CongregacaoOrigemId,
			@CongregacaoDestId = CongregacaoDestId
		From 
			Carta 
		Where 
			Id = @IdCarta

		Select
			@UsuarioNome = Nome
		From
			Usuario
		Where
			Id = @UsuarioID
	
		If (@TipoCarta = 1 And @StatusCarta = 2) --TipoCarta = TRANSFERÊNCIA e StatusCarta = Finalizado
		BEGIN
			UPDATE dbo.Membro
			SET
				CongregacaoId = @CongregacaoOrigemId,
				DataAlteracao = GETDATE()
			WHERE Id = @MembroID
		
			Select @Obs = CONCAT('Carta de Transferência n.º ', @IdCarta, ' cancelada por ', @UsuarioNome) 

			DECLARE @DateNow DATETIME = GETDATE()
			EXEC SalvarObservacaoMembro
				@MembroId = @MembroID
				, @Observacao = @Obs
				, @UsuarioId = @UsuarioID
				, @DataCadastro = @DateNow
		END
		ELSE IF (@TipoCarta = 2 And @StatusCarta = 2) --TipoCarta = MUDANÇA e StatusCarta = Finalizado
		BEGIN
			DECLARE @IdSit Int, @SitAnt INT

			SELECT
				@IdSit = S.Id -- ID DA SITUAÇÃO DE MUDANÇA
			FROM SituacaoMembro S 
			WHERE 
				S.MembroId = @MembroId 
				AND S.Id = (SELECT MAX(X.Id) FROM SituacaoMembro X WHERE X.MembroId = S.MembroId)
				AND S.Situacao = 4

			SELECT 
				@SitAnt = S.Situacao
			FROM SituacaoMembro S 
			WHERE 
				S.MembroId = @MembroId 
				AND S.Id = (SELECT MAX(X.Id) FROM SituacaoMembro X WHERE X.MembroId = S.MembroId AND X.Id < @IdSit)

			Select @Obs = CONCAT('Carta de Mudança n.º ', @IdCarta, ' cancelada por ', @UsuarioNome) 

			DECLARE @Data DATETIMEOFFSET = CAST(CONVERT(VARCHAR(10), GETDATE(), 120) AS DATETIMEOFFSET)
			EXEC dbo.SalvarSituacaoMembro
				@MembroId = @MembroId
				, @Situacao  = @SitAnt
				, @Data = @Data
				, @Observacao = @Obs
				, @SituacaoAnterior = 4

			EXEC AtualizarStatusPorSituacao @MembroId
		END

		UPDATE Carta
		SET 
			StatusCarta = 3,
			DataAlteracao = GETDATE()
		WHERE Id = @IdCarta
		
		COMMIT TRAN
	END TRY  
	BEGIN CATCH  
		SELECT  
			ERROR_NUMBER() AS ErrorNumber  
			,ERROR_SEVERITY() AS ErrorSeverity  
			,ERROR_STATE() AS ErrorState  
			,ERROR_PROCEDURE() AS ErrorProcedure  
			,ERROR_LINE() AS ErrorLine  
			,ERROR_MESSAGE() AS ErrorMessage;

		ROLLBACK TRAN
	END CATCH  
END
