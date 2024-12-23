
CREATE PROCEDURE [dbo].[ListarSituacaoMembro]
	@MembroId INT
AS
BEGIN
	SELECT 
		Id
		, MembroId
		, Situacao
		, Data
		, Observacao
		, DataCriacao
		, DataAlteracao
	FROM 
		SituacaoMembro
	WHERE
		MembroId = @MembroId
	ORDER BY 
		Data DESC, Id DESC
END