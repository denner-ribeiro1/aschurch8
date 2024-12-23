CREATE PROCEDURE [dbo].[ListarPorTipoEStatus]
	@TipoCarta INT = 0,
	@StatusCarta INT = 0,
	@UsuarioID BIGINT
AS
BEGIN
	SELECT
		C.Id
		, C.MembroId
		, M.Nome
		, M.DataRecepcao
		, C.TipoCarta
		, C.CongregacaoOrigemId
		, CO.Nome AS CongregacaoOrigem
		, CO.Cidade AS CongregacaoOrigemCidade
		, C.CongregacaoDestId
		, C.CongregacaoDest
		, ISNULL(C.Observacao, '') AS Observacao
		, C.StatusCarta
		, C.DataValidade
		, C.DataAlteracao
		, C.DataCriacao
		, C.CodigoRecebimento
		, C.TemplateId
	FROM 
		Carta C
	INNER JOIN Membro M ON C.MembroId = M.Id
	INNER JOIN Congregacao CO ON CO.Id = C.CongregacaoOrigemId
	WHERE
		(@TipoCarta = 0 OR C.TipoCarta = @TipoCarta) AND
		(@StatusCarta = 0 OR C.StatusCarta = @StatusCarta) AND
		(C.CongregacaoOrigemId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)) OR
		 C.CongregacaoDestId IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default)))
	ORDER BY
		C.Id DESC
END
