CREATE PROCEDURE RelMensalTotalCriancasApresentadas
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		COUNT(1) AS TotalCriancasApresentadas
	FROM 
		Nascimento N 
	WHERE 
		N.DataApresentacao >= @DataIni 
		AND N.DataApresentacao <= @DataFim
		AND N.CongregacaoID = @CongregacaoId
END