CREATE PROCEDURE dbo.AprovaReprovaMembro
	@MembroId INT,
	@UsuarioId INT,
	@Status INT,
	@MotivoReprovacao VARCHAR(500)
AS
BEGIN
	/*Não Aprovado*/
	IF (@Status = 4)
	BEGIN
		BEGIN TRAN
		UPDATE 
			HistoricoAprovacaoMembro
		SET 
			StatusAprovacao = 'R' 
		WHERE 
			MembroId = @MembroId AND StatusAprovacao = 'P'
		
		INSERT INTO HistoricoAprovacaoMembro(MembroId,MotivoReprovacao,UsuarioId,StatusAprovacao, DataCriacao)
		VALUES(@MembroId,@MotivoReprovacao,@UsuarioId,'P', GETDATE())

		UPDATE 
			Membro
		SET
			AprovadoPorId = @UsuarioId,
			Status = 4,
			DataAlteracao = GETDATE()
		WHERE
			Id = @MembroId
		COMMIT
	END
	/*Aprovação*/
	ELSE
	BEGIN
		BEGIN TRAN

		UPDATE 
			Membro
		SET
			AprovadoPorId = @UsuarioId,
			DataAlteracao = GETDATE()
		WHERE
			Id = @MembroId

		EXEC AtualizarStatusPorSituacao @MembroId

		UPDATE 
			HistoricoAprovacaoMembro
		SET 
			StatusAprovacao = 'A' 
		WHERE 
			MembroId = @MembroId
			AND StatusAprovacao  = 'P'
		COMMIT
	END
END
