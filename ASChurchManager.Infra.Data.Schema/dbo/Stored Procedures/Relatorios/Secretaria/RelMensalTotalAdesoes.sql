CREATE PROCEDURE RelMensalTotalAdesoes
	@Mes TINYINT, 
	@Ano INT, 
	@CongregacaoId INT
AS
BEGIN
	DECLARE @DataIni DATETIMEOFFSET, @DataFim DATETIMEOFFSET, @Total INT = 0
	EXEC Util_RetornaPeriodoMes @Mes, @Ano, @DataIni OUTPUT, @DataFim OUTPUT

	SELECT 
		@Total = COUNT(M.Nome)  
	FROM 
		Membro M 
	WHERE 
		M.DataRecepcao >= @DataIni 
		AND M.DataRecepcao <= @DataFim 
		AND M.RecebidoPor IN (1,3) /*Aclamação e Mudança*/
		AND M.CongregacaoId = @CongregacaoId 

	SELECT 
		@Total += COUNT(M.Nome) 
	FROM 
		Membro M 
	WHERE 
		M.Status = 1
		AND M.DataBatismoAguas >= @DataIni 
		AND M.DataBatismoAguas <= @DataFim 
		AND M.RecebidoPor = 2
		AND M.TipoMembro = 3
		AND M.CongregacaoId = @CongregacaoId

	SELECT @Total as TotalAdmissoes
END