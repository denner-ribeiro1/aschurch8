CREATE PROCEDURE [dbo].[ConsultarEvento]
	@Id int,
    @UsuarioID int
AS
BEGIN
	SELECT 
		E.Id
		, E.IdEventoOriginal
		, E.CongregacaoId
		, E.TipoEventoId
		, E.Descricao
		, E.DataHoraInicio
		, E.DataHoraFim
		, E.Observacoes
		, E.DataCriacao
		, E.DataAlteracao
		, C.Nome AS CongregacaoNome
		, E.Frequencia
		, E.Quantidade
		, E.AlertarEventoMesmoDia
	FROM Evento E
	LEFT JOIN Congregacao C ON C.Id = E.CongregacaoId
	Where 
		E.Id = @Id
END