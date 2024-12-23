CREATE FUNCTION [dbo].[VerificaAnoBissexto]
(
	@AnoaVerificar INT
)
RETURNS BIT
AS
BEGIN
    IF ((@AnoaVerificar % 4 = 0) AND (NOT(@AnoaVerificar % 100 = 0))) OR (@AnoaVerificar % 400 = 0)
        RETURN 1
    RETURN 0    
END