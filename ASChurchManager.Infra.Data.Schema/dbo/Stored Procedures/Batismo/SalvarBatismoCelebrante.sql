CREATE PROCEDURE [dbo].[SalvarBatismoCelebrante]
	@BatismoId INT, 
    @MembroId INT
AS
BEGIN
	INSERT INTO BatismoCelebrante 
	SELECT @BatismoId, @MembroId
END
