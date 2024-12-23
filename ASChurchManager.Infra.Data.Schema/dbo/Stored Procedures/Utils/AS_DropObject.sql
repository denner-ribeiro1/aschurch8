
CREATE PROCEDURE dbo.AS_DropObject  
	@ObjectName VARCHAR(250), @ObjectType CHAR(1) = NULL 
AS
BEGIN

   SET NOCOUNT ON

   DECLARE @Return BIT, @SQL VARCHAR(1000), @WhereName VARCHAR(255)
	  , @object   VARCHAR(255)
	  , @username VARCHAR(255)
	  , @dbname   VARCHAR(255)
	  , @temporary BIT

   CREATE TABLE #Results ( ObjectID INT, RawData VARCHAR(256), xType VARCHAR(20) )

   SELECT @object   = parsename( @ObjectName, 1 )
		 ,@username = CASE WHEN PATINDEX('%#%',@object) > 0 THEN '' ELSE ISNULL(parsename( @ObjectName, 2 ) , user_name()) END
		 ,@dbname   = CASE WHEN PATINDEX('%#%',@object) > 0 THEN 'tempdb' ELSE ISNULL(parsename( @ObjectName, 3 ),db_name()) END
		 ,@temporary = CASE WHEN PATINDEX('%##%',@object) > 0 THEN 2 WHEN PATINDEX('%#%',@object) > 0 THEN 1 ELSE 0 END

   IF @temporary = 1
   BEGIN
	  INSERT #Results (ObjectID, RawData, xType) 
	  SELECT object_id('tempdb..' + @object) AS ID, so.Name, 'U' AS xType
	  FROM tempdb..sysobjects so WITH (nolock)
	  WHERE ID = object_id('tempdb..' + @object)

	  SELECT @SQL = 'SELECT TOP 1 ID, Name, xType FROM tempdb..sysObjects WITH (nolock) WHERE Name = ''' + RawData + ''' AND xType = ''U'' '
	  FROM #Results
   END
   ELSE
   BEGIN
	  SELECT @SQL = 'SELECT TOP 1 ID, Name, xType FROM ' + @dbName + '..sysObjects WITH (nolock) WHERE Name = ''' + @object + ''' AND ( ''' + ISNULL(@ObjectType,'') + ''' = '' '' OR ''' + ISNULL(@ObjectType,'') + ''' <> '' '' AND xType = ''' + ISNULL(@ObjectType,'') + ''' ) '
	  INSERT #Results EXECUTE ( @SQL )
   END

   SELECT @Return = ISNULL((SELECT 1 FROM #Results),0)
   IF @Return = 1
   BEGIN
	  UPDATE #Results SET 
		 xType = CASE xType
			WHEN 'Fn' THEN 'FUNCTION'
			WHEN 'U'  THEN 'TABLE'
			WHEN 'P'  THEN 'PROCEDURE'
			WHEN 'V'  THEN 'VIEW'
			WHEN 'TR'  THEN 'TRIGGER'
		 END,
		 RawData = CASE 
			WHEN xType in ('Fn','P','V','T') THEN @username + '.' + @object
			WHEN xType = 'U' AND @temporary = 0 THEN @dbname + '.' + @username + '.' + RawData
			WHEN xType = 'U' AND @temporary = 1 THEN RawData
		 END
	  
	  SELECT @SQL = 'IF EXISTS ( ' + @SQL + ') DROP ' + xType + ' ' + @ObjectName FROM #Results
	  EXECUTE ( @SQL )
   END 

   RETURN @Return

END