CREATE PROCEDURE RelMensalReconciliacao
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		M.Nome AS Nome, 
		S.Data AS Data
	FROM 
		SituacaoMembro S 
	INNER JOIN Membro M ON S.MembroId = M.Id 
	WHERE 
		Data >= @DataIni 
		AND Data <= @DataFim 
		AND SituacaoAnterior in (2,3) /*2 - Disciplinado / 3 - Desviado*/
		AND M.CongregacaoId = @CongregacaoId
END