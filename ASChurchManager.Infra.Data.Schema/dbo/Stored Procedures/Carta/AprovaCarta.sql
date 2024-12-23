CREATE PROCEDURE [dbo].[AprovaCarta]
	@Id BIGINT
	, @UsuarioID BIGINT
AS
BEGIN
	DECLARE @CongregacaoDestId INT, @MembroID INT, @TipoCarta INT, @Obs VARCHAR(300)
	SELECT 
		@MembroID = MembroId
		, @CongregacaoDestId = CongregacaoDestId
		, @TipoCarta = TipoCarta
	FROM 
		Carta
	WHERE 
		Id = @Id
	
	IF (@TipoCarta = 1) -- Transferência
	BEGIN
		/*Atualizo o membro*/
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

		SELECT  @Obs = 'Transferido por Carta - n.º ' + CONVERT(VARCHAR, @Id)
		DECLARE @DateNow DATETIME = GETDATE()
		EXEC SalvarObservacaoMembro
			@MembroId = @MembroID
			, @Observacao = @Obs
			, @UsuarioId = @UsuarioID
			, @DataCadastro = @DateNow
	END
	ELSE IF (@TipoCarta = 2) -- Mudança
	BEGIN
		--Caso o tipo da Carta for igual a Mudança ja desativarei o Membro
		UPDATE 
			Membro
		SET
			Status = 5 -- Mudou-se
			, DataAlteracao = GETDATE()
			, CartaId = @Id /*Utilizo essa campo para diferenciar a mudança por carta de transferencia e mudança por situação*/
		WHERE
			Id = @MembroID

		/*Se for obreiro, removo da congregação*/
		DELETE 
			CongregacaoObreiro
		WHERE 
			MembroId = @MembroID
			AND Dirigente = 0

		DECLARE @SitAtual INT
		SELECT 
			@SitAtual = S.Situacao 
		FROM 
			SituacaoMembro S 
		WHERE 
			S.MembroId = @MembroId 
			AND S.Id = (SELECT MAX(X.Id) FROM SituacaoMembro X WHERE X.MembroId = S.MembroId)
			
		DECLARE @Data DATETIMEOFFSET = CAST(CONVERT(VARCHAR(10), GETDATE(), 120) AS DATETIMEOFFSET)
		SELECT  @Obs = 'Mudou-se com Carta de Transferência - n.º ' + CONVERT(VARCHAR, @Id)
		EXEC dbo.SalvarSituacaoMembro
			@MembroId = @MembroID
			, @Situacao  = 4
			, @Data = @Data
			, @Observacao = @Obs
			, @SituacaoAnterior = @SitAtual

	END
	--Ajuste no status da Carta
	UPDATE 
		dbo.Carta
	SET 
		StatusCarta = 2,
		DataAprovacao = GETDATE()
	WHERE 
		Id = @Id
END