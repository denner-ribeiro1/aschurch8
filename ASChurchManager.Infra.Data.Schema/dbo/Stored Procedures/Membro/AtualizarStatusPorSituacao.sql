CREATE PROCEDURE [dbo].[AtualizarStatusPorSituacao]
	@MembroId INT
AS
BEGIN
	/*Ultima situacao do Membro*/
	DECLARE @Situacao INT = 0
	SELECT TOP 1 
		@Situacao = Situacao 
	FROM 
		SituacaoMembro
	WHERE 
		MembroId = @MembroId
	ORDER BY 
		Data DESC, Id DESC

	UPDATE 
		M
	SET 
		M.Status = CASE 
					WHEN @Situacao <= 1 THEN 1 /******* Comunhao(1)- Status = 1 ********/
					WHEN @Situacao IN (2, 3, 6) THEN 2 /******* Disciplinado(2)/Desviado(3)/Abandono(6)- Status = 2 ********/
					WHEN @Situacao = 4 THEN 5 /******* Mudou(4)- Status = 5 ********/
					WHEN @Situacao = 5 THEN 6 /******* Falecido(5) - Status = 6 ********/
				 END
		, M.DataAlteracao = CONVERT(VARCHAR, GETDATE(),112)
	FROM 
		dbo.Membro M
	WHERE 
		M.Id = @MembroId

	IF (@Situacao IN (4, 5))
	BEGIN
		EXEC DeleteObreiroNaCongregacao @MembroId
	END
END