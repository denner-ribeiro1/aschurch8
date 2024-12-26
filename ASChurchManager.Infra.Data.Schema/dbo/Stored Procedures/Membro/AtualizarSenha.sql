CREATE PROCEDURE [dbo].[AtualizarSenha]
	@id INT,
	@novaSenha VARCHAR(1000)
AS
BEGIN 
	UPDATE Membro SET Senha = @novaSenha WHERE Id = @id
END