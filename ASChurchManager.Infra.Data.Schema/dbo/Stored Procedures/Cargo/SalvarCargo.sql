CREATE PROCEDURE SalvarCargo
    @Id INT
	, @Descricao VARCHAR(100)
	, @Lider BIT
	, @Obreiro BIT
	, @TipoCarteirinha TINYINT
	, @Confradesp BIT
	, @CGADB BIT
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @CargoID INT = ISNULL(@Id,0)

    IF (@Id > 0)
    BEGIN 

        UPDATE dbo.Cargo
        SET 
            Descricao = @Descricao
			, Obreiro = @Obreiro
			, Lider = @Lider
			, TipoCarteirinha = @TipoCarteirinha
            , DataAlteracao = GETDATE()
			, Confradesp = @Confradesp
			, CGADB = @CGADB
        WHERE 
            Id = @Id

    END
    ELSE
    BEGIN 

        INSERT INTO dbo.Cargo
        (
			Descricao
			, Obreiro
			, Lider
			, TipoCarteirinha
		    , DataCriacao
            , DataAlteracao
			, Confradesp
			, CGADB
		)
        SELECT
           @Descricao
		   , @Obreiro
		   , @Lider
		   , @TipoCarteirinha
		   , GETDATE()
		   , GETDATE()
		   , @Confradesp
		   , @CGADB

		SELECT @CargoID = SCOPE_IDENTITY()

    END

	SELECT @CargoID
	RETURN @CargoID

END