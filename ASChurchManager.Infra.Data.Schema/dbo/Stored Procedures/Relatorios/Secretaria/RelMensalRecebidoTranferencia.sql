CREATE PROCEDURE RelMensalRecebidoTranferencia
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		M.Nome AS Nome, 
		C.DataAprovacao AS Data 
	FROM 
		Carta C 
	INNER JOIN Membro M ON C.MembroId = M.Id
	WHERE 
		C.DataAprovacao >= @DataIni 
		AND C.DataAprovacao <= @DataFim 
		AND M.CongregacaoId = @CongregacaoId
		AND C.TipoCarta = 1 /*TRANSFERENCIA*/
END