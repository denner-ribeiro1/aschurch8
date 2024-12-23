CREATE PROCEDURE RelMensalSaidaPor
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		M.Nome AS Nome
		, S.Observacao AS Motivo 
		, S.Data AS Data
	FROM
		 SituacaoMembro S 
	INNER JOIN Membro M ON S.MembroId = M.Id 
	WHERE 
		Data >= @DataIni 
		AND Data <= @DataFim 
		AND Situacao IN (3, 4) /*3 - Desviado / 4 - Mudou-se */ 
		AND M.CongregacaoId = @CongregacaoId
END