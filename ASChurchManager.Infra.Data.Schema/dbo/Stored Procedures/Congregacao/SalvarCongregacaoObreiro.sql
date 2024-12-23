CREATE PROCEDURE SalvarCongregacaoObreiro
	@CongregacaoId INT
	, @MembroId INT
	, @Dirigente BIT NULL
AS
BEGIN	
	BEGIN
		INSERT INTO CongregacaoObreiro(CongregacaoId, MembroId, Dirigente)
		SELECT @CongregacaoId, @MembroId, ISNULL(@Dirigente, 0)
	END
END