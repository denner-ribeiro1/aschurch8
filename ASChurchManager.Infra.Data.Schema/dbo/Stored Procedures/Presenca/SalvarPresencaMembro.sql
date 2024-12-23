CREATE PROCEDURE [dbo].[SalvarPresencaInscricao]
	@Id INT,
    @PresencaId INT, 
    @MembroId INT, 
    @Nome VARCHAR(100) , 
    @CPF VARCHAR(15), 
    @Igreja VARCHAR(100), 
    @Cargo VARCHAR(100),
    @Pago BIT,
    @Usuario INT,
    @ArquivoId INT,
    @CongregacaoId INT
AS
BEGIN    
    IF EXISTS(SELECT TOP 1 * FROM PresencaInscricao WHERE Id = @Id)
    BEGIN
        UPDATE 
            PresencaInscricao
        SET
            PresencaId = @PresencaId,
            MembroId = @MembroId,
            Nome = @Nome,
            CPF = @CPF,
            Igreja = @Igreja,
            Cargo = @Cargo,
            Pago = @Pago,
            Usuario = @Usuario,
            DataAlteracao = GETDATE(),
            ArquivoId = @ArquivoId,
            CongregacaoId = @CongregacaoId
        WHERE
            Id = @Id
    END
    ELSE
    BEGIN
        INSERT INTO PresencaInscricao(PresencaId,MembroId,Nome,CPF,Igreja,Cargo,Pago,Usuario,DataCriacao,ArquivoId,CongregacaoId)
        VALUES (@PresencaId,@MembroId,@Nome,@CPF,@Igreja,@Cargo,@Pago,@Usuario,GETDATE(),@ArquivoId,@CongregacaoId)

        SELECT @Id = SCOPE_IDENTITY()
    END

    SELECT @Id
	RETURN @Id
END