CREATE PROCEDURE [dbo].[ListarObservacaoMembro]
	@MembroId int
AS
	SELECT 
		O.*,
		U.Nome
	FROM
		ObservacaoMembro O
	LEFT JOIN Usuario U ON U.Id = O.UsuarioId 
	WHERE
		O.MembroId = @MembroId
	ORDER BY DataCadastro DESC
RETURN 0
