CREATE PROCEDURE [dbo].[AlterarCongregacaoMembro]
	@MembroID INT
	, @CongregacaoDestId INT
	, @UsuarioID BIGINT
AS
BEGIN
	UPDATE 
		dbo.Membro
	SET
		CongregacaoId = @CongregacaoDestId,
		DataAlteracao = GETDATE()
	WHERE
		Id = @MembroID

	/*Se for obreiro, ajusto a congregação*/
	IF (EXISTS(SELECT TOP 1 1 FROM CongregacaoObreiro WHERE MembroId = @MembroID AND Dirigente = 0))
	BEGIN
		DELETE 
			CongregacaoObreiro
		WHERE 
			MembroId = @MembroID
			AND Dirigente = 0
		Exec SalvarCongregacaoObreiro @CongregacaoDestId, @MembroId, 0
	END
	
	DECLARE @Obs VARCHAR(100) 
	SELECT @Obs = 'Transferido sem carta por ' + Nome FROM Usuario WHERE ID = @UsuarioID

	DECLARE @DateNow DATETIME = GETDATE()
	EXEC SalvarObservacaoMembro
		@MembroId = @MembroID
		, @Observacao = @Obs
		, @UsuarioId = @UsuarioID
		, @DataCadastro = @DateNow
END
