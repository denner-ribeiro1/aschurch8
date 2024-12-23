CREATE PROCEDURE RelMensalTotalRecMudanca
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		COUNT(M.Nome) AS TotalRecMudanca 
	FROM 
		Membro M 
	WHERE 
		M.DataRecepcao >= @DataIni 
		AND M.DataRecepcao <= @DataFim 
		AND M.RecebidoPor = 3 /*MUDANÇA*/
		AND M.TipoMembro = 3
		AND M.Status = 1
		AND M.CongregacaoId = @CongregacaoId
END