CREATE PROCEDURE ListarEventosObrigatorio
	@DataHoraInicio DATETIMEOFFSET, 
	@DataHoraFim DATETIMEOFFSET,
	@Congregacao INT,
	@Frequencia TINYINT, 
	@Quantidade INT
AS
BEGIN
	DECLARE @CodCongregSede INT, @ACHOU BIT = 0,
		@DataHoraInicioNova DATETIMEOFFSET, @DataHoraFimNova DATETIMEOFFSET, @SEQ INT

	SELECT @CodCongregSede = Id FROM Congregacao WHERE Sede = 1

	IF OBJECT_ID('TEMPDB..#EVENTOS') IS NOT NULL 
		DROP TABLE #EVENTOS
	CREATE TABLE #EVENTOS (
		TipoEvento TINYINT,
		Id BIGINT,
		Descricao VARCHAR (500),
		DataHoraInicio DATETIMEOFFSET (7),
		DataHoraFim DATETIMEOFFSET (7),
	) 	

	IF (@CodCongregSede <> @Congregacao)
	BEGIN
		IF OBJECT_ID('TEMPDB..#DATAS') IS NOT NULL 
			DROP TABLE #DATAS
		CREATE TABLE #DATAS(
			Seq INT IDENTITY(1,1)
			, DataIni DATETIMEOFFSET
			, DataFin DATETIMEOFFSET
		)

		INSERT INTO #DATAS(DataIni,DataFin)
		SELECT @DataHoraInicio, @DataHoraFim

		SET @SEQ = 1
		WHILE (@SEQ < @Quantidade)
		BEGIN
			IF (@Frequencia = 2) --DIARIO
			BEGIN
				SELECT
					@DataHoraInicioNova = DATEADD(DAY, @SEQ, @DataHoraInicio),
					@DataHoraFimNova = DATEADD(DAY, @SEQ, @DataHoraFim)
			END	
			ELSE IF (@Frequencia = 3) --SEMANAL
			BEGIN
				SELECT
					@DataHoraInicioNova = DATEADD(WEEK, @SEQ, @DataHoraInicio),
					@DataHoraFimNova = DATEADD(WEEK, @SEQ, @DataHoraFim)
			END	
			ELSE IF (@Frequencia = 4) --MENSAL
			BEGIN
				SELECT
					@DataHoraInicioNova = DATEADD(MONTH, @SEQ, @DataHoraInicio),
					@DataHoraFimNova = DATEADD(MONTH, @SEQ, @DataHoraFim)
			END	
			ELSE IF (@Frequencia = 5) --ANUAL
			BEGIN
				SELECT
					@DataHoraInicioNova = DATEADD(YEAR, @SEQ, @DataHoraInicio),
					@DataHoraFimNova = DATEADD(YEAR, @SEQ, @DataHoraFim)
			END

			INSERT INTO #DATAS(DataIni,DataFin)
			SELECT @DataHoraInicioNova, @DataHoraFimNova

			SELECT @SEQ = @SEQ + 1
		END
	
		SELECT @SEQ = 1,@Quantidade = MAX(Seq) FROM #DATAS
		WHILE (@SEQ <= @Quantidade)
		BEGIN
			SELECT 
				@DataHoraInicioNova = DataIni,
				@DataHoraFimNova = DataFin
			FROM #DATAS
			WHERE Seq = @SEQ
		
			INSERT INTO #EVENTOS(TipoEvento, Id, Descricao, DataHoraInicio, DataHoraFim)
			SELECT 1, Id, Descricao, DataHoraInicio, DataHoraFim
			FROM Evento
			WHERE 
				AlertarEventoMesmoDia = 1
				AND (@DataHoraInicioNova BETWEEN DataHoraInicio AND DataHoraFim
					 OR 
					 @DataHoraFimNova BETWEEN DataHoraInicio AND DataHoraFim
					 OR
					 DataHoraInicio BETWEEN @DataHoraInicioNova AND @DataHoraFimNova
					 OR
					 DataHoraFim BETWEEN @DataHoraInicioNova AND @DataHoraFimNova)

			INSERT INTO #EVENTOS(TipoEvento, Id, Descricao, DataHoraInicio, DataHoraFim)
			SELECT 3, Id, 'Batismo', DataBatismo, NULL
			FROM Batismo 
			WHERE
				Status = 1
				AND CONVERT(date, DataBatismo) = CONVERT(date, @DataHoraInicioNova)  

			SELECT @SEQ = @SEQ + 1
		END
	END

	SELECT * FROM #EVENTOS
END