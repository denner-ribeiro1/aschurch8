CREATE PROC ConsultarRotinaPorAreaController
	@Area VARCHAR(50)
	, @Controller VARCHAR(50)
AS
BEGIN
	SELECT 
		R.Id
		, R.Area
		, R.AreaDescricao
		, R.AreaIcone
		, R.Controller
		, R.Action
		, R.MenuDescricao
		, R.MenuIcone
		, R.DataCriacao
		, R.DataAlteracao
		, R.SubMenuDescricao
		, R.SubMenuIcone
	FROM 
		dbo.Rotina R
	WHERE 
		R.Area = @Area
		And R.Controller = @Controller
	ORDER BY
		R.MenuIcone, R.Id
END