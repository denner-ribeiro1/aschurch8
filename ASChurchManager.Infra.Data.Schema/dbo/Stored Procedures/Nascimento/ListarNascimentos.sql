CREATE PROCEDURE dbo.ListarNascimentos
	@UsuarioID INT
AS
BEGIN
	SELECT 
		N.Id, 
		N.NomePai,
		N.NomeMae,
		N.Crianca,
		N.Sexo,
		N.CongregacaoID,
		N.DataNascimento,
		N.DataApresentacao,
		N.DataCriacao,
		N.DataAlteracao,
		N.Pastor,
		N.PastorId,
		C.Nome AS CongregacaoNome
	FROM 
		dbo.Nascimento N
		Left Join Congregacao C On C.Id = N.CongregacaoID
	WHERE
		CongregacaoId IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
	ORDER BY
		N.DataApresentacao DESC
END