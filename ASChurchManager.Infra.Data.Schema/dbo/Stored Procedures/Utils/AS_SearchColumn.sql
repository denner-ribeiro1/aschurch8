
CREATE PROCEDURE dbo.AS_SearchColumn   
(  
	@ColumnName sysname  
	, @CurrentDB int = 0  
	, @TempTable VARCHAR(200) = NULL  
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
 CREATE TABLE #Results (DBName varchar(30), xtype varchar(2), ObjectName sysname, ColumnName sysname)  
  
 if @CurrentDB = 1  
  BEGIN  
  
	SELECT @strQuery = 'SELECT DB_NAME(), O.xtype, O.name, C.name ' + @CRLF +  
	'FROM sysobjects O ' + @CRLF +  
	'INNER JOIN syscolumns C ON O.id = C.id ' + @CRLF +  
	'WHERE C.NAME LIKE ''' + @ColumnName + ''''  
  
--   INSERT INTO #Results  
--   EXEC(@strQuery)  

   SET @strQuery = 'INSERT INTO #Results ' + @strQuery
   EXEC dbo.AS_ExecuteTry @strQuery

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
	 SELECT @strQuery = 'SELECT ''' + dbname + ''', O.xtype, O.name, C.name ' + @CRLF +  
	  'FROM [' + dbname + ']..sysobjects O ' + @CRLF +  
	  'INNER JOIN [' + dbname + ']..syscolumns C ON O.id = C.id ' + @CRLF +  
	  'WHERE C.NAME LIKE ''' + @ColumnName + ''''  
	 FROM #databases   
	 WHERE RowID = @ctMax  
  
--     INSERT INTO #Results  
--     EXEC(@strQuery)  
	 SET @strQuery = 'INSERT INTO #Results ' + @strQuery
	 EXEC dbo.AS_ExecuteTry @strQuery
  
	 SELECT @ctMax = @ctMax - 1  
	END  
  
  END  
  
 IF @TempTable IS NULL  
 BEGIN  
  SELECT DBName, xtype, ObjectName, ColumnName  
  FROM #Results   
  ORDER BY DBName, xtype, ObjectName, ColumnName  
 END  
 ELSE  
 BEGIN  
  DECLARE @InsertSQL VARCHAR(8000)  
  SELECT @InsertSQL =  
  '  
  INSERT '+ @TempTable +' ( DBName, xtype, ObjectName, ColumnName )   
  SELECT DBName, xtype, ObjectName, ColumnName  
  FROM #Results   
  ORDER BY DBName, xtype, ObjectName, ColumnName  
  '  
--  EXEC (@InsertSQL)  

  EXEC dbo.AS_ExecuteTry @InsertSQL

 END  
   
  
END