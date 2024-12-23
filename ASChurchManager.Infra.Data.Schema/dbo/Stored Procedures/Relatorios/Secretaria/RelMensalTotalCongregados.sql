CREATE PROCEDURE RelMensalTotalCongregados
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	/*RETORNA O TOTAL DE CONGREGADOS POR CONGREGAÇÃO QUE FORAM CADASTRADOS ATÉ O ULTIMO DIA DO MES*/
	SELECT 
		COUNT(1) AS TotalCongregados 
	FROM 
		Membro M 
	WHERE 
		M.TipoMembro IN (1, 2) /*CONGREGADO E CANDIDATO AO BATISMO*/
		AND Status = 1 /*ATIVO*/
		AND M.CongregacaoId = @CongregacaoId
		AND M.DataCriacao <= @DataFim
END