CREATE FUNCTION [dbo].[SplitString]
(
	@String varchar(8000),
    @Delimiter char(1)
)
RETURNS @VALUES TABLE (
        SEQ INT IDENTITY(1, 1),
        CAMPO VARCHAR(50)
    )
AS     
BEGIN
    IF SUBSTRING(@String, LEN(@String), 1) <> @Delimiter
        SET @String += @Delimiter

    DECLARE @idx INT     
    DECLARE @slice VARCHAR(8000)     

    SELECT @idx = 1     
    IF LEN(@String) < 1 OR ISNULL(@String, '') = ''
        RETURN 

    WHILE (@idx <> 0)
    BEGIN     
        SET @idx = CHARINDEX(@Delimiter, @String)     
        IF @idx <> 0 
        BEGIN
            DECLARE @VALOR VARCHAR(100)
            SET @VALOR = LEFT(@String, @idx - 1) 
            IF ISNULL(@VALOR, '') <> ''
                INSERT INTO @values(CAMPO) SELECT LEFT(@String, @idx - 1)        
        END
        SET @String = RIGHT(@String, LEN(@String) - @idx)     
        IF LEN(@String) = 0 
            BREAK
    END 
    RETURN  
END