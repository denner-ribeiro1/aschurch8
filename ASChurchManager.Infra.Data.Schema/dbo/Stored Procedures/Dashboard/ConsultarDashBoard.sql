CREATE PROCEDURE [dbo].[ConsultarDashBoard]
	@UsuarioId int = 0
AS
BEGIN
	DECLARE @CONGR TABLE (CongregacaoId INT NOT NULL)  
	INSERT INTO @CONGR
	SELECT * FROM dbo.CongregacaoAcesso(@UsuarioId, default)

	/*Valores Gráfico*/
	SELECT 
		COUNT(1) AS QTD, STATUS
	FROM
		Membro
	WHERE
		CongregacaoId IN (SELECT * FROM @CONGR)
		AND TipoMembro = 3
	GROUP BY 
		Status 
	ORDER BY 
		Status

	SELECT
		ISNULL(COUNT(1),0) AS QUANTIDADECARTASPENDENTES
	FROM 
		Carta C
	WHERE
		C.TipoCarta = 1
		AND C.StatusCarta = 1 
		AND	(C.CongregacaoOrigemId  IN (SELECT * FROM @CONGR) 
			 OR C.CongregacaoDestId IN (SELECT * FROM @CONGR))

	SELECT 
		ISNULL(COUNT(1), 0) AS QUANTIDADECONGREGADOS
	FROM
		Membro
	WHERE
		TipoMembro = 1
		AND CongregacaoId IN (SELECT * FROM @CONGR)
END