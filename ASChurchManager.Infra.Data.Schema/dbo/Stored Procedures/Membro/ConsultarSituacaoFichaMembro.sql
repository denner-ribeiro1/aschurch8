CREATE PROCEDURE dbo.ConsultarSituacaoFichaMembro
	@Id INT	
AS
BEGIN
	SELECT
		CONVERT(VARCHAR, S.Situacao) AS Situacao
		, Data AS Data
		, Observacao
	FROM
		SituacaoMembro S
	WHERE
		S.MembroId = @Id
	ORDER BY
		Data DESC
END