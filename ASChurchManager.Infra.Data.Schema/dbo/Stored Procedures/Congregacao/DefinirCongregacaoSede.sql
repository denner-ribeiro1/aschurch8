CREATE PROCEDURE dbo.DefinirCongregacaoSede
	@CongregacaoId INT
AS
BEGIN

	IF EXISTS 
	(
		SELECT TOP 1 1
		FROM dbo.Congregacao
		WHERE Sede = 1
	)
	BEGIN

		RAISERROR('Já existe uma Congregação definida como Sede', 16,1)

	END
	ELSE
	BEGIN

		UPDATE C
		SET 
			Sede = 1
		FROM dbo.Congregacao C
		WHERE ID = @CongregacaoId

		SELECT @CongregacaoId
		RETURN @CongregacaoId

	END

END