CREATE PROCEDURE SalvarCursoArquivoMembro
	@Id INT
	,@MembroId INT
	,@TipoArquivo TINYINT
	,@Descricao VARCHAR(200)
    ,@CursoId INT
    ,@Local VARCHAR(50)
    ,@NomeCurso VARCHAR(50)
    ,@DataInicioCurso DATETIMEOFFSET
    ,@DataEncerramentoCurso DATETIMEOFFSET
    ,@CargaHoraria INT
    ,@NomeOriginal VARCHAR(100)
    ,@Tamanho INT
	,@NomeArmazenado VARCHAR(100) OUTPUT
AS
BEGIN
	IF (@Id > 0)
	BEGIN
		SELECT @NomeArmazenado = CONVERT(VARCHAR, @Id) + '_' + CONVERT(VARCHAR, @MembroId) + '_' + @NomeOriginal

		UPDATE 
			CursoArquivoMembro
		SET 
			MembroId = @MembroId
			,TipoArquivo = @TipoArquivo
			,Descricao = @Descricao
			,CursoId = @CursoId
			,Local = @Local
			,NomeCurso = @NomeCurso
			,DataInicioCurso = @DataInicioCurso
			,DataEncerramentoCurso = @DataEncerramentoCurso
			,CargaHoraria = @CargaHoraria
			,NomeArmazenado = @NomeArmazenado
			,NomeOriginal = @NomeOriginal
			,Tamanho = @Tamanho
			,DataCriacao = GETDATE()
		WHERE 
			Id = @ID
	END
	ELSE
	BEGIN
		INSERT INTO CursoArquivoMembro(
			MembroId
			,TipoArquivo
			,Descricao
			,CursoId
			,Local
			,NomeCurso
			,DataInicioCurso
			,DataEncerramentoCurso
			,CargaHoraria
			,NomeArmazenado
			,NomeOriginal
			,Tamanho
			,DataCriacao
		)
		VALUES(
			@MembroId
		   ,@TipoArquivo
		   ,@Descricao
           ,@CursoId
           ,@Local
           ,@NomeCurso
           ,@DataInicioCurso
           ,@DataEncerramentoCurso
           ,@CargaHoraria
           ,@NomeArmazenado
           ,@NomeOriginal
           ,@Tamanho
		   ,GETDATE()
		)
		
		SELECT @Id = SCOPE_IDENTITY()

		SELECT @NomeArmazenado = CONVERT(VARCHAR, @Id) + '_' + CONVERT(VARCHAR, @MembroId) + '_' + @NomeOriginal
		UPDATE 
			CursoArquivoMembro
		SET 
			NomeArmazenado =  @NomeArmazenado
		WHERE 
			Id = @Id
	END

	SELECT @Id
	RETURN @Id	
END