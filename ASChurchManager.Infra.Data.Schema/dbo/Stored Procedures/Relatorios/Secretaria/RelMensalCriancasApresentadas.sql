CREATE PROCEDURE RelMensalCriancasApresentadas
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT
		N.Crianca AS NomeCrianca
		, N.DataNascimento AS DataNascimento
		, N.NomePai
		, N.NomeMae
		, N.Pastor AS PastorOficiante
		, N.DataApresentacao AS DataApresentacaoCrianca
	FROM 
		Nascimento N 
	WHERE 
		N.DataApresentacao >= @DataIni 
		AND N.DataApresentacao <= @DataFim
		AND N.CongregacaoID = @CongregacaoId
END