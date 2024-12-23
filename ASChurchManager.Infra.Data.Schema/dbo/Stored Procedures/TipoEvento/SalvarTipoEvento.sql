
CREATE PROC SalvarTipoEvento
    @Id INT, 
	@Descricao VARCHAR(100)

AS
BEGIN

DECLARE @TipoEventoID INT = isnull(@Id,0)

    IF (@Id > 0 )
    BEGIN 
        UPDATE 
            dbo.TipoEvento
        SET 
            Descricao = @Descricao,
            DataAlteracao = GETDATE()
        WHERE 
            Id = @Id
    END
    ELSE
    BEGIN 
        INSERT INTO dbo.TipoEvento
           (Descricao,
		    DataCriacao,
            DataAlteracao)
        SELECT
           @Descricao,
		   GETDATE(),
		   GETDATE()

		   SELECT @TipoEventoID = SCOPE_IDENTITY()
    END

	SELECT @TipoEventoID
	RETURN @TipoEventoID
END