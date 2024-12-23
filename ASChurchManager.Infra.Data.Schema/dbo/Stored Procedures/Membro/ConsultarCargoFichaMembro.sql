CREATE PROCEDURE dbo.ConsultarCargoFichaMembro
	@Id INT	
AS
BEGIN
	SELECT
		C.Descricao AS Cargo
		, CM.LocalConsagracao
		, CM.DataCargo
		, CM.Confradesp
		, CM.CGADB
	FROM
		CargoMembro CM
		INNER JOIN Cargo C ON CM.CargoId = C.Id
	WHERE
		CM.MembroId = @Id
	ORDER BY
		CM.DataCargo DESC
END