CREATE PROCEDURE RelMensalTotalRecAclamacao
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		COUNT(1) AS TotalRecAclamacao 
	FROM 
		Membro M 
	WHERE 
		M.DataRecepcao >= @DataIni 
		AND M.DataRecepcao <= @DataFim 
		AND M.RecebidoPor = 1 /*ACLAMAÇÃO*/
		AND M.TipoMembro = 3
		AND M.Status = 1
		AND M.CongregacaoId = @CongregacaoId
END