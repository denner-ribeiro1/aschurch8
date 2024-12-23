CREATE PROCEDURE [dbo].[AtualizarPagoPresencaInscricao]
    @Id INT, 
    @Pago BIT
AS
BEGIN
    UPDATE
        PresencaInscricao
    SET
        Pago = @Pago
    WHERE
        Id = @Id
END