CREATE FUNCTION [dbo].[CongregacaoAcesso]
(
	@UsuarioID INT,
	@IncSede BIT = 0
)
RETURNS @Congregacao TABLE
(
	CongregacaoId INT
)
AS
BEGIN
	DECLARE @CongregacaoIDUsuario INT
	SELECT
		@CongregacaoIDUsuario = CongregacaoId
	FROM
		Usuario
	WHERE
		Id = @UsuarioID

	IF @UsuarioID > 0 AND NOT EXISTS(SELECT TOP 1 Id FROM Congregacao WHERE Id = @CongregacaoIDUsuario AND Sede = 1)
	BEGIN
		INSERT @Congregacao
		SELECT Id FROM Congregacao WHERE
			Id = @CongregacaoIDUsuario OR 
			CongregacaoResponsavelId = @CongregacaoIDUsuario

		IF (@IncSede = 1)
		BEGIN
		INSERT @Congregacao
			SELECT Id FROM Congregacao WHERE Sede = 1
		END
	END
	ELSE
	BEGIN
		INSERT @Congregacao
		SELECT 
			Id
		FROM
			Congregacao
	END
	RETURN
END