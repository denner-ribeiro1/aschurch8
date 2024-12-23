CREATE PROCEDURE SalvarCongregacaoGrupo
	@CongregacaoId INT
	, @GrupoId INT
	, @NomeGrupo VARCHAR(100)
	, @ResponsavelId INT
AS
BEGIN	
	BEGIN
		INSERT INTO CongregacaoGrupo(CongregacaoId, GrupoId, NomeGrupo, ResponsavelId, DataCriacao)
		SELECT @CongregacaoId, @GrupoId, @NomeGrupo, @ResponsavelId, GETDATE()
	END
END