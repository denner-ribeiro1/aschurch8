CREATE  PROCEDURE ConsultarCarta
    @Id INT,
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
		, C.Observacao
		, C.StatusCarta
		, C.DataValidade
		, C.DataAlteracao
		, C.DataCriacao
		, C.CodigoRecebimento
		, C.TemplateId
		, CA.Descricao AS Cargo
		, CM.CONFRADESP
		, CM.CGADB
	FROM 
		Carta C
	INNER JOIN Membro M ON C.MembroId = M.Id
	INNER JOIN Congregacao CO ON CO.Id = C.CongregacaoOrigemId
	LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id) 
	LEFT JOIN Cargo CA ON CM.CargoId = CA.Id
	WHERE 
		C.Id = @Id AND 
		(C.CongregacaoOrigemId IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)) OR
		 C.CongregacaoDestId IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default)))
END