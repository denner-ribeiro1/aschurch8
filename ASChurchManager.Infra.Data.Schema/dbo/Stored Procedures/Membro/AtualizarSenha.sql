CREATE PROCEDURE [dbo].[AtualizarSenha]
	@id INT,
	@novaSenha VARCHAR(1000),
	@atualizarSenha BIT
AS
BEGIN 
	UPDATE 
		Membro 
	SET 
		Senha = @novaSenha,
		AtualizarSenha = @atualizarSenha 
	WHERE 
		Id = @id
END