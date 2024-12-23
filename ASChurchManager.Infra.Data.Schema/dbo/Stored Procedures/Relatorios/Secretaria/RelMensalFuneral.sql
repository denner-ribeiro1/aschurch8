CREATE PROCEDURE RelMensalFuneral
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		M.Nome as Nome
		, S.Data AS Data 
	FROM 
		SituacaoMembro S 
	INNER JOIN Membro M ON S.MembroId = M.Id
	WHERE 
		S.Data >= @DataIni 
		AND S.Data <= @DataFim
		AND S.Situacao = 5
		AND M.Status = 6
		AND M.CongregacaoId = @CongregacaoId
END