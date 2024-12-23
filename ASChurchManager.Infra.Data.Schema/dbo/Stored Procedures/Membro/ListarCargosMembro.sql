CREATE PROCEDURE [dbo].[ListarCargosMembro]
	@MembroId INT
AS
BEGIN
	SET NOCOUNT ON
	SELECT 
		CM.Id
		, C.Id AS CargoId
		, C.Descricao
		, CM.DataCargo
		, C.TipoCarteirinha
		, CM.LocalConsagracao
		, CM.Confradesp
		, CM.CGADB
	FROM CargoMembro CM
	INNER JOIN dbo.Cargo C ON CM.CargoId = C.Id
	WHERE
		MembroId = @MembroId
	ORDER BY CM.DataCargo DESC
END