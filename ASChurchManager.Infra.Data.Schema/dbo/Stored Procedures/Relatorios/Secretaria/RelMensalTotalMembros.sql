CREATE PROCEDURE RelMensalTotalMembros
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	/*RETORNA O TOTAL DE MEMBROS POR CONGREGAÇÃO QUE FORAM CADASTRADOS ATÉ O ULTIMO DIA DO MES*/
	SELECT 
		COUNT(1) AS TotalMembros 
	FROM 
		Membro M 
	WHERE 
		M.CongregacaoId = @CongregacaoId
		AND M.DataCriacao <= @DataFim
		AND Status = 1 /*Ativo*/
		AND TipoMembro = 3 /*Membro*/
END