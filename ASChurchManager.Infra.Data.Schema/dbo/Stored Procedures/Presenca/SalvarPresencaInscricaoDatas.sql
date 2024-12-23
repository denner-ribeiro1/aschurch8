CREATE PROCEDURE [dbo].[SalvarPresencaInscricaoDatas]
	@InscricaoId INT,
	@DataId INT,
	@Situacao TINYINT,
    @Tipo TINYINT, 
	@Justificativa VARCHAR(100)
AS
BEGIN
	IF EXISTS(SELECT TOP 1 1 FROM PresencaInscricaoDatas WHERE InscricaoId = @InscricaoId AND DataId = @DataId )
	BEGIN
		UPDATE 
			PresencaInscricaoDatas
		SET
			Situacao = @Situacao
			, DataAlteracao = GETDATE()
			, Tipo = @Tipo
			, Justificativa = @Justificativa
		WHERE 
			InscricaoId = @InscricaoId
			AND DataId = @DataId
	END
	ELSE
	BEGIN
		INSERT INTO PresencaInscricaoDatas(InscricaoId,DataId,Situacao,Tipo,Justificativa,DataAlteracao)
		VALUES (@InscricaoId, @DataId, @Situacao, @Tipo, @Justificativa, GETDATE())
	END
END
