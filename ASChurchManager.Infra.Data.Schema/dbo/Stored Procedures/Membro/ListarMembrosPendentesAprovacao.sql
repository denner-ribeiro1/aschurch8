
CREATE PROCEDURE dbo.ListarMembrosPendentesAprovacao
	@UsuarioId INT
AS
BEGIN
	
	SET NOCOUNT ON
 
	SELECT 
		M.Id
		, M.Nome
		, CongregacaoNome = C.Nome 
		, M.DataCriacao
	FROM dbo.Membro M
	INNER JOIN dbo.Congregacao C ON M.CongregacaoId = C.Id
	WHERE 
		Status = 3 -- 3: PendenteAprovacao
		AND C.Id IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
 
END