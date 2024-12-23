﻿CREATE PROC ConsultarRotinas
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
		, R.SubMenuDescricao
		, R.SubMenuIcone
		, R.DataCriacao
		, R.DataAlteracao
	FROM 
		dbo.Rotina R
	ORDER BY
		R.MenuIcone, R.Id
END