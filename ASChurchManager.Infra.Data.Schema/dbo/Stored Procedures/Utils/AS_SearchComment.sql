
CREATE PROCEDURE dbo.AS_SearchComment  
(  
	@Text varchar(300)  
	, @CurrentDB int = 0  
)  
AS  
BEGIN  
 SET NOCOUNT ON  
   
 declare @strQuery varchar(1000)  
	, @CRLF char(2)  
	, @ctMax int  
	, @dbname sysname  
  
 SELECT  @CRLF   = CHAR(13) + CHAR(10)  
  
 EXEC sp_DropObject #Results   
 CREATE TABLE #Results (DBName varchar(30), ObjectName sysname, xtype varchar(2))  
  
 if @CurrentDB = 1  
  BEGIN  
  
	SELECT @strQuery = 'SELECT DB_NAME(), O.name, O.xtype ' + @CRLF +  
	'FROM sysobjects O ' + @CRLF +  
	'INNER JOIN syscomments C ON O.id = C.id ' + @CRLF +  
	'WHERE C.text LIKE ''%' + @Text + '%'''  
  
--   INSERT INTO #Results  
--   EXEC(@strQuery)
  
   SET @strQuery = 'INSERT INTO #Results ' + @strQuery
   EXEC dbo.ExecuteTry @strQuery
  
  END  
 else  
  BEGIN  
   EXEC sp_DropObject #databases  
  
   SELECT RowID = IDENTITY(int,1,1), dbname = name  
   INTO #databases  
   FROM master..sysdatabases  
  
   SELECT @ctMax = MAX(RowID) FROM #databases  
  
   while @ctMax > 0  
	BEGIN  
	 SELECT @strQuery = 'SELECT ''' + dbname + ''', O.name, O.xtype ' + @CRLF +  
	  'FROM [' + dbname + ']..sysobjects O ' + @CRLF +  
	  'INNER JOIN [' + dbname + ']..syscomments C ON O.id = C.id ' + @CRLF +  
	  'WHERE C.text LIKE ''%' + @Text + '%'''  
	 FROM #databases   
	 WHERE RowID = @ctMax  
  
--     INSERT INTO #Results  
--     EXEC(@strQuery)  
 
	 SET @strQuery = 'INSERT INTO #Results ' + @strQuery
	 EXEC dbo.ExecuteTry @strQuery
  
	 SELECT @ctMax = @ctMax - 1  
	END  
  
  END  
  
 SELECT DISTINCT DBName, xtype, ObjectName  
 FROM #Results   
 ORDER BY DBName, ObjectName  
  
END