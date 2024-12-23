CREATE PROCEDURE [dbo].[ListarHistoricoTrans]
	  @MembroId INT
AS
BEGIN
	SELECT 
		G1.Nome as CongregacaoOrigem, 
		G2.Nome as CongregacaoDestino, 
		C.DataCriacao as DataDaTransferencia
	FROM 
		Carta C
	INNER JOIN Congregacao G1 ON C.CongregacaoOrigemId = G1.Id
	INNER JOIN Congregacao G2 ON C.CongregacaoDestId = G2.Id
	WHERE
		MembroId = @MembroId AND 
		StatusCarta = 2 AND 
		TipoCarta = 1 
	ORDER BY 
		C.DataCriacao DESC
END
