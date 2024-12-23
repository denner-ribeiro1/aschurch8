CREATE PROCEDURE SalvarGrupo
    @Id INT
	, @Descricao VARCHAR(100)
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @GrupoID INT = ISNULL(@Id,0)

    IF (@Id > 0)
    BEGIN 

        UPDATE dbo.Grupo
        SET 
            Descricao = @Descricao
            , DataAlteracao = GETDATE()
        WHERE 
            Id = @Id

    END
    ELSE
    BEGIN 

        INSERT INTO dbo.Grupo
        (
			Descricao
		    , DataCriacao
            , DataAlteracao
		)
        SELECT
           @Descricao
		   , GETDATE()
		   , GETDATE()

		SELECT @GrupoID = SCOPE_IDENTITY()

    END

	SELECT @GrupoID
	RETURN @GrupoID

END