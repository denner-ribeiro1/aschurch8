CREATE PROCEDURE SalvarCurso
	@Id INT
	, @Descricao VARCHAR(100)
    , @DataInicio DATETIMEOFFSET(7)
    , @DataEncerramento DATETIMEOFFSET(7)
    , @CargaHoraria INT
AS
BEGIN
	IF (@Id > 0)
	BEGIN
		UPDATE Curso
		SET 
			Descricao = @Descricao
			,DataInicio = @DataInicio
			,DataEncerramento = @DataEncerramento
			,CargaHoraria = @CargaHoraria
			,DataAlteracao = GETDATE()
		WHERE 
			Id = @Id
	END
	ELSE
	BEGIN
		INSERT INTO Curso
			   (Descricao
			   ,DataInicio
			   ,DataEncerramento
			   ,CargaHoraria
			   ,DataCriacao
			   ,DataAlteracao)
		 VALUES
			   (@Descricao
			   ,@DataInicio
			   ,@DataEncerramento
			   ,@CargaHoraria
			   ,GETDATE()
			   ,GETDATE())
		SELECT @Id = SCOPE_IDENTITY()
	END

	RETURN @Id
END