CREATE PROCEDURE [dbo].[SalvarObservacaoMembro]
    @MembroId INT, 
	@Observacao VARCHAR(500), 
    @UsuarioId INT,
	@DataCadastro DATETIMEOFFSET
AS
BEGIN	
	INSERT INTO ObservacaoMembro(MembroId, Observacao, UsuarioId, DataCadastro)
	SELECT @MembroId, @Observacao, @UsuarioId, @DataCadastro
END
