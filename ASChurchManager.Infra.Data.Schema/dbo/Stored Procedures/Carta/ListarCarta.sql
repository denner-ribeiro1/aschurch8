
CREATE PROCEDURE ListarCarta
	@UsuarioID INT
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
		(C.CongregacaoOrigemId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)) OR
		 C.CongregacaoDestId IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default)))
END	