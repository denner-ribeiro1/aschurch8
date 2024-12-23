
CREATE PROCEDURE SalvarPresenca
    @Id INT = 0 
    , @Descricao VARCHAR(255) 
    , @TipoEventoId INT
    , @DataMaxima DATETIME
    , @Valor FLOAT 
    , @ExclusivoCongregacao BIT 
    , @NaoMembros BIT
    , @GerarEventos BIT
    , @InscricaoAutomatica BIT
    , @CongregacaoId INT 
    , @Status TINYINT
AS
BEGIN
    IF EXISTS (SELECT TOP 1 1 FROM dbo.Presenca WHERE Id = @Id)
	BEGIN
        UPDATE
            Presenca
        SET
            Descricao = @Descricao
            , TipoEventoId = @TipoEventoId
            , Valor = @Valor
            , DataMaxima = @DataMaxima
            , ExclusivoCongregacao = @ExclusivoCongregacao
            , NaoMembros = @NaoMembros
            , GerarEventos = @GerarEventos 
            , InscricaoAutomatica = @InscricaoAutomatica
            , CongregacaoId = @CongregacaoId
            , [Status] = @Status
            , DataAlteracao = GETDATE()
        WHERE
            Id = @Id
        
        SELECT @Id
    END
    ELSE
    BEGIN
        INSERT INTO Presenca(Descricao, TipoEventoId, Valor, DataMaxima, ExclusivoCongregacao, NaoMembros, GerarEventos, InscricaoAutomatica, CongregacaoId, [Status], DataCriacao)
        VALUES(@Descricao, @TipoEventoId, @Valor, @DataMaxima, @ExclusivoCongregacao, @NaoMembros, @GerarEventos, @InscricaoAutomatica, @CongregacaoId, @Status, GETDATE())
        
        SELECT @Id = SCOPE_IDENTITY()
    END
    SELECT @Id
	RETURN @Id
END