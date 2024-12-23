CREATE PROCEDURE RelMensalTotalDemissoes
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET, @Total INT = 0
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT
	
	/*saída por*/
	SELECT 
		@Total = COUNT(M.Nome) 
	FROM 
		SituacaoMembro S 
	INNER JOIN Membro M ON S.MembroId = M.Id 
	WHERE 
		Data >= @DataIni 
		AND Data <= @DataFim 
		AND Situacao IN (3, 4) /*3 - Desviado / 4 - Mudou-se */ 
		AND M.CongregacaoId = @CongregacaoId

	/*Saída por TRANSFERENCIA/MUDANÇA*/
	SELECT 
		@Total += COUNT(M.Nome) 
	FROM 
		Carta C 
	INNER JOIN Membro M ON C.MembroId = M.Id
	WHERE 
		C.DataAprovacao >= @DataIni 
		AND C.DataAprovacao <= @DataFim
		AND C.TipoCarta IN (1,2)
		AND C.CongregacaoOrigemId = @CongregacaoId
		AND C.StatusCarta = 2 /*CARTA FINALIZADA*/

	SELECT @Total as TotalDemissoes
END