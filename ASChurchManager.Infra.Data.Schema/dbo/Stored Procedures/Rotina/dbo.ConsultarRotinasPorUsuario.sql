CREATE  PROCEDURE dbo.ConsultarRotinasPorUsuario
	@UsuarioId BIGINT
AS
BEGIN

	SET NOCOUNT ON

	-- DECLARE @UsuarioId BIGINT = 1

	IF OBJECT_ID('#Menus') IS NOT NULL
	BEGIN
		DROP TABLE #Menus
	END

	CREATE TABLE #Menus
	(
		[Id] INT NOT NULL,
		[Area] VARCHAR (50)  NOT NULL,
		[AreaDescricao] VARCHAR (50)  NULL,
		[AreaIcone] VARCHAR (50)  NULL,
		[Controller] VARCHAR (50)  NOT NULL,
		[Action] VARCHAR(50) NOT NULL,
		[MenuDescricao] VARCHAR (50)  NULL,
		[MenuIcone] VARCHAR (50)  NULL,
		[SubMenuDescricao] VARCHAR (50)  NULL,
		[SubMenuIcone] VARCHAR (50)  NULL,
		[DataCriacao] DATETIMEOFFSET NULL,
		[DataAlteracao] DATETIMEOFFSET NULL,	
	)

	INSERT INTO #Menus
	(
		Id
		, Area
		, AreaDescricao
		, AreaIcone
		, Controller
		, Action
		, MenuDescricao
		, MenuIcone
		, SubMenuDescricao
		, SubMenuIcone
		, DataCriacao
		, DataAlteracao
	)
	SELECT DISTINCT
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
	FROM dbo.Rotina R
	INNER JOIN dbo.PerfilRotina PR ON R.Id = PR.IdRotina
	INNER JOIN dbo.Perfil P ON PR.IdPerfil = P.Id
	INNER JOIN dbo.Usuario U ON P.Id = U.PerfilId
	WHERE
		U.Id = @UsuarioId
	ORDER BY
		R.MenuIcone, R.Id

	IF EXISTS
	(
		SELECT TOP 1 1 FROM #Menus
	)
	BEGIN
		
		SELECT * FROM #Menus

	END
	ELSE
	BEGIN

		SELECT DISTINCT
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
		FROM dbo.Rotina R
	
	END

END