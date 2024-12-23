CREATE PROC SalvarRotina
	@Id INT
	, @Area VARCHAR(50)
	, @AreaDescricao VARCHAR(50)
	, @AreaIcone VARCHAR(50)
	, @Controller VARCHAR(50)
	, @Action VARCHAR(50)
	, @MenuDescricao VARCHAR(50)
	, @MenuIcone VARCHAR(50)
	, @SubMenuDescricao VARCHAR(50)
	, @SubMenuIcone VARCHAR(50)
AS
BEGIN
	IF (@Id = 0)
	BEGIN
		IF (NOT EXISTS(SELECT TOP 1 * FROM Rotina R WHERE R.Area = @Area And R.Controller = @Controller And R.Action = @Action))
		BEGIN
			INSERT INTO dbo.Rotina (
				Area
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
			 VALUES (
				@Area
				, @AreaDescricao
				, @AreaIcone
				, @Controller
				, @Action
				, @MenuDescricao
				, @MenuIcone
				, @SubMenuDescricao
				, @SubMenuIcone
				, CONVERT(VARCHAR, GETDATE(), 112)
				, CONVERT(VARCHAR, GETDATE(), 112))
			SELECT @Id = SCOPE_IDENTITY()
		END
		ELSE
		BEGIN
			UPDATE 
				Rotina
			SET 
				Area = @Area
				, AreaDescricao = @AreaDescricao
				, AreaIcone = @AreaIcone
				, Controller = @Controller
				, Action = @Action
				, MenuDescricao = @MenuDescricao
				, MenuIcone = @MenuIcone
				, SubMenuDescricao = @SubMenuDescricao
				, SubMenuIcone = @SubMenuIcone
				, DataAlteracao = CONVERT(VARCHAR, GETDATE(), 112)
			WHERE 
				 Area = @Area And Controller = @Controller And Action = @Action
		END
	END
	ELSE
	BEGIN
		UPDATE 
			Rotina
		SET 
			Area = @Area
			, AreaDescricao = @AreaDescricao
			, AreaIcone = @AreaIcone
			, Controller = @Controller
			, Action = @Action
			, MenuDescricao = @MenuDescricao
			, MenuIcone = @MenuIcone
			, SubMenuDescricao = @SubMenuDescricao
			, SubMenuIcone = @SubMenuIcone
			, DataAlteracao = CONVERT(VARCHAR, GETDATE(), 112)
		WHERE 
			Id = @Id
	END
	SELECT @Id 
	RETURN @Id 
END