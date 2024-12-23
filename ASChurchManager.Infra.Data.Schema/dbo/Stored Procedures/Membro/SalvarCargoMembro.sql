CREATE PROCEDURE [dbo].SalvarCargoMembro
	@MembroId INT
	, @CargoId INT
	, @DataCargo DATETIMEOFFSET
	, @LocalConsagracao VARCHAR(50)
	, @Confradesp VARCHAR(15)
	, @CGADB VARCHAR(15)
AS
BEGIN
	INSERT INTO CargoMembro(MembroId, CargoId, DataCargo, LocalConsagracao, Confradesp, CGADB, DataCriacao)
	SELECT @MembroId, @CargoId, @DataCargo, @LocalConsagracao, @Confradesp, @CGADB, GETDATE()
END