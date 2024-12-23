CREATE PROCEDURE ConsultarCodigoRecebimento
    @IdMembro INT	
AS 

BEGIN

	SELECT
		CodigoRecebimento
	FROM Carta
	WHERE
		MembroId = @IdMembro

END