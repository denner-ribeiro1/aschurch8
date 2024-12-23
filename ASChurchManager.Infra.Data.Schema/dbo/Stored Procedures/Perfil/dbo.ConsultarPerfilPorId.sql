
CREATE PROCEDURE dbo.ConsultarPerfilPorId
	@Id INT
	, @UsuarioID INT
AS
BEGIN
	
	SET NOCOUNT ON

	SELECT DISTINCT
		PerfilId = P.Id 
		, P.Nome
		, P.TipoPerfil
		, P.Status
		, P.DataCriacao
		, P.DataAlteracao
	FROM dbo.Perfil P
	WHERE P.Id = @Id

	SELECT
		RotinaId = R.Id
		, R.Area
		, R.AreaDescricao
		, R.AreaIcone
		, R.Controller
		, R.Action
		, R.MenuDescricao
		, R.MenuIcone
		, R.SubMenuDescricao
	FROM dbo.PerfilRotina PR 
	LEFT JOIN dbo.Rotina R ON PR.IdRotina = R.Id
	WHERE PR.IdPerfil = @Id

END