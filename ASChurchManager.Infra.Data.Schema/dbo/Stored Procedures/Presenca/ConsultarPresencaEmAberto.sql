CREATE PROCEDURE [dbo].[ConsultarPresencaEmAberto]
	@UsuarioID INT
AS
BEGIN
	SELECT 
		P.Id
		, P.Descricao
		, P.TipoEventoId
		, T.Descricao AS DescrTipoEventoId
		, P.DataMaxima
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
		, (SELECT MIN(D.DataHoraInicio) FROM PresencaDatas D WHERE D.PresencaId = P.Id) AS DataHoraInicio
	FROM Presenca P
	INNER JOIN TipoEvento T ON T.Id = P.TipoEventoId
	INNER JOIN Congregacao C ON C.Id = P.CongregacaoId
	WHERE 
		P.Status = 1 And 
		P.ExclusivoCongregacao = 0
	UNION
	SELECT 
		P.Id
		, P.Descricao
		, P.TipoEventoId
		, T.Descricao AS DescrTipoEventoId
		, P.DataMaxima
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
		, (SELECT MIN(D.DataHoraInicio) FROM PresencaDatas D WHERE D.PresencaId = P.Id) AS DataHoraInicio
	FROM Presenca P
	INNER JOIN TipoEvento T ON T.Id = P.TipoEventoId
	INNER JOIN Congregacao C ON C.Id = P.CongregacaoId
	WHERE 
		P.Status = 1 
		AND P.ExclusivoCongregacao = 1 
		AND P.CongregacaoId IN (SELECT  * FROM dbo.CongregacaoAcesso(@UsuarioID, default))
END