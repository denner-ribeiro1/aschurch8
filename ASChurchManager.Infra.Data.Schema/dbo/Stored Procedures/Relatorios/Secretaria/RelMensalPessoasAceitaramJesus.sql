CREATE PROCEDURE RelMensalPessoasAceitaramJesus
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	/*RETORNA O TOTAL DE CONGREGADOS POR CONGREGAÇÃO QUE FORAM CADASTRADOS NO MES*/
	SELECT 
		COUNT(M.Nome) AS TotalNovosConvertidos 
	FROM 
		Membro M 
	WHERE 
		M.TipoMembro = 1 
		AND Status = 1
		AND M.DataCriacao >= @DataIni
		AND M.DataCriacao <= @DataFim
		AND M.CongregacaoId = @CongregacaoId
END
