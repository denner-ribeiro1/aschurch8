CREATE PROCEDURE [dbo].[ConsultarTodasPresenca]
	@UsuarioID INT
AS
BEGIN
	IF OBJECT_ID('tempdb..#EVENTOS') IS NOT NULL DROP TABLE #EVENTOS
	SELECT 
		P.Id
		, P.Descricao
		, P.TipoEventoId
		, T.Descricao AS DescrTipoEventoId
		, P.DataMaxima
		, (SELECT MIN(D.DataHoraInicio) FROM PresencaDatas D WHERE D.PresencaId = P.Id) AS DataHoraInicio
		, P.Valor
		, P.ExclusivoCongregacao
		, P.NaoMembros
		, P.GerarEventos
		, P.InscricaoAutomatica
		, P.CongregacaoId
		, C.Nome AS Congregacao
		, P.Status AS [Status]
		, P.DataAlteracao
		, P.DataCriacao
	INTO #EVENTOS
	FROM Presenca P
	INNER JOIN TipoEvento T ON T.Id = P.TipoEventoId
	INNER JOIN Congregacao C ON C.Id = P.CongregacaoId
	WHERE
		P.ExclusivoCongregacao = 1 AND
		C.Id IN (SELECT * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
	UNION
	SELECT 
		P.Id
		, P.Descricao
		, P.TipoEventoId
		, T.Descricao AS DescrTipoEventoId
		, P.DataMaxima
		, (SELECT MIN(D.DataHoraInicio) FROM PresencaDatas D WHERE D.PresencaId = P.Id) AS DataHoraInicio
		, P.Valor
		, P.ExclusivoCongregacao
		, P.NaoMembros
		, P.GerarEventos
		, P.InscricaoAutomatica
		, P.CongregacaoId
		, C.Nome AS Congregacao
		, P.Status AS [Status]
		, P.DataAlteracao
		, P.DataCriacao
	FROM Presenca P
	INNER JOIN TipoEvento T ON T.Id = P.TipoEventoId
	INNER JOIN Congregacao C ON C.Id = P.CongregacaoId
	WHERE
		P.ExclusivoCongregacao = 0


	SELECT * FROM #EVENTOS

	SELECT D.* FROM PresencaDatas D
	INNER JOIN #EVENTOS E ON E.Id = D.PresencaId
END
