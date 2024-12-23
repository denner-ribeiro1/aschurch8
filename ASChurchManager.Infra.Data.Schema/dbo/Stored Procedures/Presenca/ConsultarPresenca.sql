CREATE PROCEDURE [dbo].[ConsultarPresenca]
	@Id int = 0
AS
BEGIN
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
		P.Id = @Id

	SELECT * FROM PresencaDatas
	WHERE PresencaId = @Id
END
