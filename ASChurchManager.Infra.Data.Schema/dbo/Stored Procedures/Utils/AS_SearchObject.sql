
CREATE PROCEDURE dbo.AS_SearchObject   
(  
	@ObjectName SYSNAME  
	, @xtype varchar(2) = NULL  
	, @Specific BIT = 0  
	, @FirstFoundObject VARCHAR(700) = NULL OUTPUT  
) 
AS  
BEGIN  
 SET NOCOUNT ON  
   
 DECLARE @object varchar(255)  
  , @dbname varchar(255)  
  , @strQuery varchar(1000)  
  , @strWhere varchar(100)  
  , @CRLF char(2)  
  , @ctMax int  
   
 SELECT @object = PARSENAME(@ObjectName, 1)  
		, @dbname = PARSENAME(@ObjectName, 3)  
  , @strWhere = ''  
  , @CRLF = CHAR(13) + CHAR(10)  
   
 CREATE TABLE #Results ( DBName varchar(30), ObjectName sysname, xtype varchar(2) )  
-- DECLARE @Results TABLE ( DBName varchar(30), ObjectName sysname, xtype varchar(2) )  
   
 IF @xtype IS NOT NULL  
  SELECT @strWhere = 'AND xtype = ''' + @xtype + ''''  
 ELSE  
  SELECT @strWhere = 'AND xtype in (''U'',''V'',''P'',''FN'',''IF'',''TF'',''TR'')'  
   
 IF @dbname IS NOT NULL  
 BEGIN  
   SELECT @strQuery = 'SELECT ''' + @dbname + ''',name,xtype' + @CRLF  
	 + 'FROM [' + @dbname + ']..sysobjects WHERE name '  
   , @strQuery = @strQuery + CASE @Specific WHEN 0 THEN 'LIKE ''%' + @object + '%''' ELSE '= ''' + @object + '''' END  
	+ @CRLF + @strWhere  
	
--  INSERT INTO #Results  
--  EXEC(@strQuery)   ---------------------------------------------------
  
   SET @strQuery = 'INSERT INTO #Results ' + @strQuery
   EXEC dbo.AS_ExecuteTry @strQuery

 END  
 ELSE  
 BEGIN  
  SELECT RowID = IDENTITY(int,1,1), dbname = name  
  INTO #databases  
  FROM master..sysdatabases  
  --WHERE status < 2048  
	
  SELECT @ctMax = MAX(RowID) FROM #databases  
	
  WHILE @ctMax > 0  
  BEGIN  
   SELECT @strQuery = 'SELECT ''' + dbname + ''',name,xtype' + @CRLF  
	 + 'FROM [' + dbname + ']..sysobjects WHERE name '  
	, @strQuery = @strQuery + CASE @Specific WHEN 0 THEN 'LIKE ''%' + @object + '%''' ELSE '= ''' + @object + '''' END  
	 + @CRLF + @strWhere  
   FROM #databases   
   WHERE RowID = @ctMax  
	 
--   INSERT INTO #Results 
--   EXEC(@strQuery)  

   SET @strQuery = 'INSERT INTO #Results ' + @strQuery
   EXEC dbo.AS_ExecuteTry @strQuery
	 
   SELECT @ctMax = @ctMax - 1  
  END  
 END  
   
 IF @FirstFoundObject IS NULL  
  SELECT DBName, xtype, ObjectName  
  FROM #Results   
  ORDER BY DBName, xtype, ObjectName  
   
 SELECT TOP 1 @FirstFoundObject = DBName + '.' + xtype + '.' + ObjectName  
 FROM #Results   
 ORDER BY DBName, xtype, ObjectName  
END