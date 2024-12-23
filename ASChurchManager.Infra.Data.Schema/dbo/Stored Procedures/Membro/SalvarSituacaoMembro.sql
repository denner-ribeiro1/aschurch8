CREATE PROCEDURE [dbo].SalvarSituacaoMembro
	 @MembroId INT
	, @Situacao INT
	, @Data SMALLDATETIME
	, @Observacao VARCHAR(200)
	, @SituacaoAnterior INT = NULL
AS
BEGIN
	INSERT INTO SituacaoMembro(MembroId, Situacao, SituacaoAnterior, Data, Observacao, DataCriacao, DataAlteracao)
	SELECT @MembroId, @Situacao, ISNULL(@SituacaoAnterior, @Situacao), @Data, @Observacao, GETDATE(), GETDATE()
END
