CREATE PROCEDURE SalvarConviteEvento
	@EventoId int,
	@CongregacaoId int,
	@GrupoId int,
	@ConvidadoId int
AS
BEGIN
	INSERT INTO ConviteEvento
		(EventoId
		,CongregacaoId
		,GrupoId
		,ConvidadoId)
    VALUES
		(@EventoId
		,@CongregacaoId
		,@GrupoId
		,@ConvidadoId)
END
