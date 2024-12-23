CREATE PROCEDURE [dbo].[RelatorioHistoricoTrans]
	@DataInicio SMALLDATETIME, 
	@DataFinal SMALLDATETIME,
	@Congregacao INT,
	@UsuarioID INT
AS
BEGIN
	SELECT 
		CO.Nome as CongregacaoOrigem, 
		CASE WHEN C.TipoCarta = 2 THEN 'Mudança' ELSE CD.Nome END as CongregacaoDestino, 
		CASE C.TipoCarta 
			WHEN 1 THEN 'Transferência'
			WHEN 2 THEN 'Mudança'
			WHEN 3 THEN 'Recomendação'
		END AS TipoCarta,
		C.DataCriacao as DataDaTransferencia,
		M.Nome,
		CASE C.StatusCarta 
			WHEN 1 THEN 'Aguardando Recebimento'
			WHEN 2 THEN 'Finalizado'
			WHEN 3 THEN 'Cancelado'
		END AS StatusCarta
	FROM 
		Carta C
		INNER JOIN Membro M ON C.MembroId = M.Id
		INNER JOIN Congregacao CO ON CO.Id = C.CongregacaoOrigemId
		LEFT JOIN Congregacao CD ON CD.Id = C.CongregacaoDestId
	WHERE 
		C.TipoCarta IN (1, 2) --TRANSFERENCIA E MUDANÇA
		AND C.DataCriacao BETWEEN @DataInicio AND @DataFinal 
		AND (ISNULL(@Congregacao, 0) = 0 OR C.CongregacaoOrigemId = @Congregacao OR C.CongregacaoDestId = @Congregacao) 
		AND (C.CongregacaoOrigemId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)) OR 
		     C.CongregacaoDestId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)))
	ORDER BY C.DataCriacao DESC
END