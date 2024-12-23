CREATE PROCEDURE [dbo].[DeletarEvento]
	@Id int,
	@ExcluirVinc bit = 0
AS
BEGIN
	BEGIN TRAN
	IF (@ExcluirVinc = 1)
	BEGIN
		DECLARE @IdEventoOrig INT
		SELECT 
			@IdEventoOrig = ISNULL(IdEventoOriginal, Id)
		FROM
			Evento 
		WHERE
			Id = @Id
		
		DELETE 
			Evento 
		WHERE 
			IdEventoOriginal = @IdEventoOrig

		DELETE 
			Evento 
		WHERE 
			Id = @IdEventoOrig
	END
	ELSE
	BEGIN
		DELETE 
			Evento 
		WHERE 
			Id = @Id
	END
	
	COMMIT
END