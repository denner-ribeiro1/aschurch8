CREATE PROCEDURE dbo.ConsultarFichaMembro
	@Id INT	
AS
BEGIN
	EXEC ConsultarDadosMembroFichaMembro @Id

	EXEC ConsultarSituacaoFichaMembro @Id

	EXEC ConsultarCargoFichaMembro @Id

	EXEC ConsultarObservacaoFichaMembro @Id

	EXEC ConsultarHistoricoFichaMembro @Id

	EXEC ConsultarPresencaFichaMembro @Id

END