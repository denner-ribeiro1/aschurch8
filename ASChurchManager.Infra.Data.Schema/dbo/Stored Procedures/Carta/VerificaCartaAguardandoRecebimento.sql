CREATE PROCEDURE VerificaCartaAguardandoRecebimento
	@MembroId INT
AS 
BEGIN
	SET NOCOUNT ON

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
		C.MembroId = @MembroId AND
		C.StatusCarta = 1 AND 
		C.TipoCarta = 1
END