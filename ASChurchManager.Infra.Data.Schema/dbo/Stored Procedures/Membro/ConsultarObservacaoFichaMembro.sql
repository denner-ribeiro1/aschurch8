CREATE PROCEDURE dbo.ConsultarObservacaoFichaMembro
	@Id INT	
AS
BEGIN
	SELECT 
		O.Observacao
		, O.DataCadastro
		, U.Nome
	FROM
		ObservacaoMembro O
		LEFT JOIN Usuario U ON U.Id = O.UsuarioId
	WHERE
		O.MembroId = @Id
	ORDER BY
		O.DataCadastro DESC
END