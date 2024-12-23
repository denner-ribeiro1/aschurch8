CREATE PROCEDURE [dbo].[SalvarCongregacaoObservacao]
	@CongregacaoId INT
	, @Observacao VARCHAR(300)
	, @UsuarioId INT
	, @DataCadastro SMALLDATETIME = NULL
AS
BEGIN	
	BEGIN
		INSERT INTO CongregacaoObservacao(CongregacaoId, Observacao, UsuarioId, DataCadastro)
		SELECT @CongregacaoId, @Observacao, @UsuarioId, ISNULL(@DataCadastro, GETDATE()) 
	END
END
