CREATE PROCEDURE RelatorioCursosMembro
	@CongregacaoId INT,
	@CursoId INT,
	@UsuarioID INT
AS
BEGIN
	Select 
		CONVERT(varchar, C.Id) + ' - ' + C.Descricao + ' - Inicio: ' + CONVERT(varchar, C.DataInicio, 103) + ' - Fim: ' + CONVERT(varchar, C.DataEncerramento, 103) As Curso, 
		CONVERT(varchar, M.Id) + ' - ' + M.Nome As Membro,
		CONVERT(varchar, CG.Id) + ' - ' + CG.Nome As Congregacao
	From CursoArquivoMembro CA
	Inner Join Curso C On CA.CursoId = C.Id
	Inner Join Membro M On M.Id = CA.MembroId
	Inner Join Congregacao CG On M.CongregacaoId = CG.Id
	Where
		CG.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default)) AND
		(ISNULL(@CursoId, 0) = 0 OR C.Id = @CursoId) AND
		(ISNULL(@CongregacaoId, 0) = 0 OR M.CongregacaoId = @CongregacaoId)
	Order By CG.Id ASC, M.Nome
END