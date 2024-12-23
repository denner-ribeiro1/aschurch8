CREATE PROCEDURE dbo.AtualizarMembroBatismo
	@Id INT,
	@Presente INT,
	@BatismoId INT
AS
BEGIN
	IF(@Presente = 1)
	BEGIN
		DECLARE @DataBatismoAguas DATETIMEOFFSET
		SELECT @DataBatismoAguas = DataBatismo From Batismo Where Id = @BatismoId

		UPDATE 
			dbo.Membro
		SET 
			Status = 1
			, TipoMembro = 3 /*Membro*/
			, DataBatismoAguas = @DataBatismoAguas
			, RecebidoPor = 2 /*Batismo*/
			, DataRecepcao = @DataBatismoAguas
			, DataAlteracao = GETDATE()
			, BatismoSituacao = 1
			, BatismoId = @BatismoId
		WHERE 
			Id = @Id

		/*CADASTRO DA SITUAÇÃO INICIAL*/
		EXEC dbo.SalvarSituacaoMembro
			@MembroId = @Id
			, @Situacao  = 1
			, @Data = @DataBatismoAguas
			, @Observacao = 'Batizado nas Águas'
	END
	ELSE
	BEGIN
		/* VOLTA O MEMBRO PRA CONGREGADO */
		UPDATE 
			dbo.Membro
		SET 
			Status = 1
			, TipoMembro = 1 /*Congregado*/
			, DataBatismoAguas = NULL
			, DataPrevistaBatismo = NULL
			, BatismoSituacao = NULL
			, BatismoId = NULL
			, DataAlteracao = GETDATE()
		WHERE 
			Id = @Id
	END

	/*Atualiza a tabela de candidatos ao Batismo*/
	Exec AlterarSituacaoCandidatoBatismo @BatismoId, @Id, @Presente
END