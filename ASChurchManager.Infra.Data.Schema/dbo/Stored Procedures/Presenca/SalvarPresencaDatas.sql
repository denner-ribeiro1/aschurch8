CREATE PROCEDURE dbo.SalvarPresencaDatas
	@Id INT
	, @PresencaId INT
	, @DataHoraInicio DATETIME
    , @DataHoraFim DATETIME
	, @CongregacaoId INT
	, @TipoEventoId INT
	, @Descricao VARCHAR(100)
	, @GerarEventos BIT 
AS
BEGIN
	IF (@Id = 0)
	BEGIN
		INSERT INTO PresencaDatas(PresencaId, DataHoraInicio, DataHoraFim, [Status])
		SELECT @PresencaId, @DataHoraInicio, @DataHoraFim,  1
	END
	ELSE
	BEGIN
		UPDATE 
			PresencaDatas
		SET
			PresencaId = @PresencaId, 
			DataHoraInicio = @DataHoraInicio, 
			DataHoraFim = @DataHoraFim, 
			EventoId = NULL
		WHERE
			Id = @Id
	END
END
