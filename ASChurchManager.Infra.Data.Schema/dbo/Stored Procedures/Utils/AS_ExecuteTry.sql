CREATE PROCEDURE dbo.AS_ExecuteTry
(
	@strSQL varchar(2000)
)  
AS  
BEGIN

	SET NOCOUNT ON  

	BEGIN TRY  
		EXEC(@strSQL)  
	END TRY  
	BEGIN CATCH  
		PRINT ERROR_MESSAGE()  
		--   PRINT 'Object not found --> [' + @strSQL + ']'  
	END CATCH
END