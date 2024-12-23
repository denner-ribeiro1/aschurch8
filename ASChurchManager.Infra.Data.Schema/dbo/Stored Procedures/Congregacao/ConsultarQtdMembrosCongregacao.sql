CREATE PROCEDURE [dbo].[ConsultarQtdMembrosCongregacao]
	@Id INT
AS
BEGIN	
	SELECT 'M' AS Tipo, COUNT(*) AS Quantidade 
	FROM Membro M
	WHERE CongregacaoId = @Id
	UNION
	SELECT 'E', COUNT(*) 
	FROM Evento 
	WHERE CongregacaoId = @Id
	UNION
	SELECT 'N', COUNT(*) 
	FROM Nascimento 
	WHERE CongregacaoId = @Id
	UNION
	SELECT 'C', COUNT(*) 
	FROM Casamento 
	WHERE CongregacaoId = @Id
	UNION
	SELECT 'O', COUNT(*) 
	FROM CongregacaoObreiro
	WHERE CongregacaoId = @Id and Dirigente = 0
	UNION
	SELECT 'P', COUNT(*) 
	FROM Presenca
	WHERE CongregacaoId = @Id
	UNION
	SELECT 'U', COUNT(*) 
	FROM Usuario
	WHERE CongregacaoId = @Id
END