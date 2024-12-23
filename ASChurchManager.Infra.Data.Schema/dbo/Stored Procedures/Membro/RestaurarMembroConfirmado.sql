CREATE PROCEDURE [dbo].[RestaurarMembroConfirmado]
	@MembroId INT,
	@Campos VARCHAR(4000),
	@UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON
	DECLARE @ERRO VARCHAR(8000)

	BEGIN TRAN	
	BEGIN TRY

		DECLARE @UltControle INT
		SELECT @UltControle = ISNULL(MAX(Controle),0) FROM dbo.HistoricoMembro WHERE Id = @MembroId

		/*HISTORICO DO MEMBRO ATUAL*/
		IF OBJECT_ID('tempdb..#HISTORICOMEMBRO') IS NOT NULL DROP TABLE #HISTORICOMEMBRO 
		SELECT
			H.Id
			, Nome
			, Cpf
			, RG
			, OrgaoEmissor
			, DataNascimento
			, NomePai
			, NomeMae
			, EstadoCivil
			, Nacionalidade
			, NaturalidadeEstado
			, NaturalidadeCidade
			, Profissao
			, TelefoneResidencial
			, TelefoneCelular
			, TelefoneComercial
			, Email
			, Logradouro
			, Numero
			, Complemento
			, Bairro
			, Cidade
			, Estado
			, Pais
			, Cep
			, Sexo
			, Escolaridade
			, Ip
		INTO #HISTORICOMEMBRO 
		FROM
			HistoricoMembro H
		WHERE
			H.Id = @MembroId
			AND H.Controle = @UltControle

		/*TABELA TEMPORARIA COM A SITUACAO DO MEMBRO ATUAL*/
		IF OBJECT_ID('tempdb..#MEMBRO') IS NOT NULL DROP TABLE #MEMBRO 
		SELECT
			Id
			, Nome
			, Cpf
			, RG
			, OrgaoEmissor
			, DataNascimento
			, NomePai
			, NomeMae
			, EstadoCivil
			, Nacionalidade
			, NaturalidadeEstado
			, NaturalidadeCidade
			, Profissao
			, TelefoneResidencial
			, TelefoneCelular
			, TelefoneComercial
			, Email
			, Logradouro
			, Numero
			, Complemento
			, Bairro
			, Cidade
			, Estado
			, Pais
			, Cep
			, Sexo
			, Escolaridade
		INTO
			#MEMBRO
		FROM
			Membro
		WHERE
			Id = @MembroId

		IF (ISNULL(@Campos, '') <> '')
		BEGIN
			/*MONTAGEM DOS CAMPOS PARA O UPDATE*/
			IF OBJECT_ID('tempdb..#CAMPOS') IS NOT NULL DROP TABLE #CAMPOS 
			SELECT 
				SEQ, 
				CAMPO 
			INTO #CAMPOS 
			FROM 
				dbo.SplitString(@Campos, ',')

			DECLARE @CmpUpdate VARCHAR(2000), @Ini INT, @Fim INT
			SELECT @CmpUpdate = '', @Ini = 1, @Fim = MAX(SEQ) FROM #CAMPOS
			WHILE @Ini <= @Fim
			BEGIN
				DECLARE @Cmp VARCHAR(100)
				SELECT @Cmp = CAMPO FROM #CAMPOS WHERE SEQ = @Ini
				SET @CmpUpdate += 'M.' + @Cmp + ' = H.' + @Cmp + ','
				SET @Ini += 1   
			END
			SELECT @CmpUpdate = SUBSTRING(@CmpUpdate, 1, LEN(@CmpUpdate) - 1)

			DECLARE @SQLQUERY NVARCHAR(4000)
			/*Atualizado o membro*/
			SELECT @SQLQUERY =  'UPDATE M SET ' + @CmpUpdate + ' FROM Membro M '
			SELECT @SQLQUERY += 'INNER JOIN #HISTORICOMEMBRO H ON M.Id = H.Id '
			EXECUTE SP_EXECUTESQL @SQLQUERY

			SELECT @SQLQUERY =  'UPDATE M SET ' + @CmpUpdate + ' FROM #HISTORICOMEMBRO M '
			SELECT @SQLQUERY += 'INNER JOIN #MEMBRO H ON H.Id = M.Id '
			EXECUTE SP_EXECUTESQL @SQLQUERY
		END
		ELSE
		BEGIN
			UPDATE 
				M
			SET 
				M.Nome = H.Nome
				, M.Cpf = H.Cpf
				, M.RG = H.RG
				, M.OrgaoEmissor = H.OrgaoEmissor
				, M.DataNascimento = H.DataNascimento
				, M.NomePai = H.NomePai
				, M.NomeMae = H.NomeMae
				, M.EstadoCivil = H.EstadoCivil
				, M.Nacionalidade = H.Nacionalidade
				, M.NaturalidadeEstado = H.NaturalidadeEstado
				, M.NaturalidadeCidade = H.NaturalidadeCidade
				, M.Profissao = H.Profissao				
				, M.TelefoneResidencial = H.TelefoneResidencial
				, M.TelefoneCelular = H.TelefoneCelular
				, M.TelefoneComercial = H.TelefoneComercial
				, M.Email = H.Email
				, M.Logradouro = H.Logradouro
				, M.Numero = H.Numero
				, M.Complemento = H.Complemento
				, M.Bairro = H.Bairro
				, M.Cidade = H.Cidade
				, M.Estado = H.Estado
				, M.Pais = H.Pais
				, M.Cep = H.Cep
				, M.Sexo = H.Sexo
				, M.Escolaridade = H.Escolaridade
			FROM Membro M
			INNER JOIN #HISTORICOMEMBRO H ON M.Id = H.Id

			UPDATE 
				M
			SET 
				M.Nome = H.Nome
				, M.Cpf = H.Cpf
				, M.RG = H.RG
				, M.OrgaoEmissor = H.OrgaoEmissor
				, M.DataNascimento = H.DataNascimento
				, M.NomePai = H.NomePai
				, M.NomeMae = H.NomeMae
				, M.EstadoCivil = H.EstadoCivil
				, M.Nacionalidade = H.Nacionalidade
				, M.NaturalidadeEstado = H.NaturalidadeEstado
				, M.NaturalidadeCidade = H.NaturalidadeCidade
				, M.Profissao = H.Profissao				
				, M.TelefoneResidencial = H.TelefoneResidencial
				, M.TelefoneCelular = H.TelefoneCelular
				, M.TelefoneComercial = H.TelefoneComercial
				, M.Email = H.Email
				, M.Logradouro = H.Logradouro
				, M.Numero = H.Numero
				, M.Complemento = H.Complemento
				, M.Bairro = H.Bairro
				, M.Cidade = H.Cidade
				, M.Estado = H.Estado
				, M.Pais = H.Pais
				, M.Cep = H.Cep
				, M.Sexo = H.Sexo
				, M.Escolaridade = H.Escolaridade
			FROM #HISTORICOMEMBRO M
			INNER JOIN #MEMBRO H ON H.Id = M.Id
		END

		/*ATUALIZACAO DO HISTORICO DO MEMBRO */
		DECLARE @Ip VARCHAR(500)
		SELECT @UltControle = @UltControle + 1
		/*MANTENHO O IP ORIGINAL Q FEZ A PRIMEIRA ALTERAÇÀO*/
		SELECT @Ip = IP FROM dbo.HistoricoMembro WHERE Id = @MembroId AND Controle = 1

		INSERT INTO dbo.HistoricoMembro
		(
			Id
			, Controle
			, Nome
			, Cpf
			, RG
			, OrgaoEmissor
			, DataNascimento
			, NomePai
			, NomeMae
			, EstadoCivil
			, Nacionalidade
			, NaturalidadeEstado
			, NaturalidadeCidade
			, Profissao
			, TelefoneResidencial
			, TelefoneCelular
			, TelefoneComercial
			, Email
			, Logradouro
			, Numero
			, Complemento
			, Bairro
			, Cidade
			, Estado
			, Pais
			, Cep
			, Sexo
			, Escolaridade
			, DataAlteracao
			, [Ip]
			, UsuarioId
		)
		SELECT
			Id
			, @UltControle
			, Nome
			, Cpf
			, RG
			, OrgaoEmissor
			, DataNascimento
			, NomePai
			, NomeMae
			, EstadoCivil
			, Nacionalidade
			, NaturalidadeEstado
			, NaturalidadeCidade
			, Profissao
			, TelefoneResidencial
			, TelefoneCelular
			, TelefoneComercial
			, Email
			, Logradouro
			, Numero
			, Complemento
			, Bairro
			, Cidade
			, Estado
			, Pais
			, Cep
			, Sexo
			, Escolaridade
			, GETDATE()
			, @Ip
			, @UsuarioId
		FROM
			#HISTORICOMEMBRO  
	COMMIT
	
    END TRY
	BEGIN CATCH
		SELECT @ERRO = 'Atualizando Membro - Código: ' + CONVERT(VARCHAR(6), ERROR_NUMBER()) + ' - ' + ERROR_MESSAGE()
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		RAISERROR (15600,-1,-1, @ERRO);
	END CATCH
	SELECT @MembroId
	RETURN @MembroId
END