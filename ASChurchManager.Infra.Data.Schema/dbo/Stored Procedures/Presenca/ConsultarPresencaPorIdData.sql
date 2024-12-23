CREATE PROCEDURE [dbo].[ConsultarPresencaPorIdData]
	@IdData INT
AS
BEGIN
	DECLARE @Id INT 
	SELECT @Id = PresencaId FROM PresencaDatas D WHERE D.Id = @IdData

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
		(ISNULL(@Id, 0) = 0 OR P.Id = @Id)
		AND EXISTS(	SELECT TOP 1 * FROM PresencaDatas D WHERE D.PresencaId = P.Id) 
	
	SELECT 
		D.* 
	FROM 
		PresencaDatas D
	WHERE 
		(ISNULL(@IdData, 0) = 0 OR D.Id = @IdData)
END
