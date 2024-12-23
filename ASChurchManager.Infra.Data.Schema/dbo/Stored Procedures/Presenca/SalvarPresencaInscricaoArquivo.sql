CREATE PROCEDURE [dbo].[SalvarPresencaInscricaoArquivo]
	@NomeArquivo VARCHAR(100)
AS
BEGIN
    DECLARE @ArquivoId INT

    INSERT INTO PresencaInscricaoArquivo(Nome,DataCaptura)
    VALUES(@NomeArquivo,GETDATE())
    
    SELECT @ArquivoId = SCOPE_IDENTITY()

    SELECT @ArquivoId 
    RETURN @ArquivoId 
END