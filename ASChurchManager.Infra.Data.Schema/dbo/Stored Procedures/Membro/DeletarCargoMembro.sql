CREATE PROCEDURE [dbo].[DeletarCargoMembro]
	@MembroId INT
AS
BEGIN
	DELETE 
		dbo.CargoMembro
	WHERE
		MembroId = @MembroId
END