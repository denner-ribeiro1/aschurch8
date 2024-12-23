CREATE PROCEDURE RelMensalTotalCriancas
	@Mes CHAR(2), 
	@Ano CHAR(4), 
	@CONGREGACAOID INT
	
	AS
BEGIN
	DECLARE @DATA1 DATETIMEOFFSET, @DATA2 DATETIMEOFFSET, @UltimoDia char(2)
	Select @Mes = REPLICATE('0', 2 - LEN(@Mes)) + RTrim(@Mes)
	SELECT @UltimoDia = CONVERT(char(2), DAY(DATEADD(d,-1,DATEADD(M,1,CONVERT(DATETIME,@Ano + @Mes + '01')))))		
	SELECT @DATA1 = @Ano +'-' + @Mes +'-'+'01'
	SELECT @DATA2 = @Ano +'-' + @Mes +'-'+ @UltimoDia

	SELECT COUNT(N.Crianca) AS TotalCriancas
	FROM Nascimento N WHERE N.CongregacaoID = @CongregacaoId
	AND (N.IdMembroMae <> 0 OR N.IdMembroPai <> 0)
	AND N.DataCriacao >= @DATA1
	AND N.DataCriacao <= @DATA2
END