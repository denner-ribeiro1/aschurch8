CREATE PROCEDURE RelMensalRecebidoCartaMudança
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		 M.Nome AS Nome, 
		 M.DataRecepcao AS Data
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