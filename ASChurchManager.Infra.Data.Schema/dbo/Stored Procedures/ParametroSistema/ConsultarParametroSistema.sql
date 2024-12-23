CREATE PROCEDURE ConsultarParametroSistema
	@Nome VARCHAR(60)
	, @Colunas VARCHAR(255) = NULL
	, @Where VARCHAR(255) = NULL
	, @Ativo BIT = 1
	, @Valor VARCHAR(60) = NULL
AS
BEGIN
	
	SET NOCOUNT ON
	
	DECLARE @Sql VARCHAR(MAX)
	
	------------ SOMENTE PARA TESTE
	--DECLARE 
	--	@Sql VARCHAR(MAX)
	--	, @Nome VARCHAR(100)		
	--	, @Where VARCHAR(255)
	--	, @Valor varchar(60)
	--	, @Ativo BIT 
	--	, @Colunas VARCHAR(255) 
		
				
	--SET @Nome = 'Religiao'
	--SET @Where = 'WHERE UF = ''SP'''
	--SET @Valor = '1'
	--SET @Ativo = 1
	------------ SOMENTE PARA TESTE
	
	SELECT 
		@Sql= 
			CASE WHEN Valor IS NOT NULL THEN 
					'SELECT Nome AS Tipo, Valor, Descricao FROM dbo.ParametroSistema WHERE Ativo = ' + CONVERT(VARCHAR, @Ativo) + ' AND Nome = ''' + ISNULL(@Nome, '''''') + ''''
			ELSE 
					CASE WHEN Tabela IS NOT NULL THEN 
						'SELECT Tipo = ''' + @Nome + ''', ' + CASE WHEN @Colunas IS NOT NULL THEN @Colunas ELSE 
										CASE WHEN Colunas IS NOT NULL THEN Colunas ELSE ' * ' 
										END 
									END + ' FROM ' + Tabela + ' ' + 
							CASE WHEN @Where IS NOT NULL THEN 
								@Where 
							ELSE 
								ISNULL(SqlWhere, '') 
							END 
							
					END
			END + 
			CASE WHEN @Valor IS NOT NULL THEN ' AND Valor = ''' + @Valor + ''''
			ELSE '' END + 
			'ORDER BY Tipo, Descricao'
			
	FROM dbo.ParametroSistema
	WHERE Nome = @Nome
	
	--PRINT @SQL
	EXEC (@Sql)

END