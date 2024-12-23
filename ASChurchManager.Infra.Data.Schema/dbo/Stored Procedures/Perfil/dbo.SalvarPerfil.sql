
CREATE PROCEDURE dbo.SalvarPerfil
	@Id INT
	, @Nome VARCHAR(60)
	, @TipoPerfil TINYINT 
	, @Status BIT
AS
BEGIN
	
	SET NOCOUNT ON

	IF NOT EXISTS
	(
		SELECT TOP 1 1
		FROM dbo.Perfil 
		WHERE Id = @Id	
	)
	BEGIN

		IF EXISTS
		(
			SELECT TOP 1 1 FROM dbo.Perfil
			WHERE LOWER(Nome) = LOWER(@Nome)
		)
		BEGIN
			
			RAISERROR ('Já existe um Perfil com esse nome.', 16, 1);

		END
		ELSE
		BEGIN

			INSERT INTO dbo.Perfil (Nome, TipoPerfil, Status, DataCriacao, DataAlteracao)
			SELECT @Nome, @TipoPerfil, 1, GETDATE(), GETDATE()
		
			SELECT @Id = SCOPE_IDENTITY()	

		END

	END
	ELSE
	BEGIN

		-- DELETA TODAS AS ROTINAS DO PERFIL
		DELETE FROM dbo.PerfilRotina
		WHERE IdPerfil = @Id

		UPDATE dbo.Perfil
		SET 
			Status = @Status,
			Nome = @Nome
		WHERE Id = @Id

	END
	
	SELECT @Id
	RETURN @Id

END