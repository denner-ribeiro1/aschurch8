CREATE PROCEDURE RelMensalTotalBatismo
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		COUNT(1) AS TotalRecebidoBatismo 
	FROM 
		Membro M 
	WHERE 
		M.DataBatismoAguas >= @DataIni 
		AND M.DataBatismoAguas <= @DataFim 
		AND M.RecebidoPor = 2 /*BATISMO*/
		AND M.TipoMembro = 3
		AND M.Status = 1
		AND M.CongregacaoId = @CongregacaoId
END