CREATE PROCEDURE dbo.DeleteObreiroNaCongregacao
	@MembroId INT
AS
BEGIN
	DELETE dbo.CongregacaoObreiro
	WHERE MembroId = @MembroId
END