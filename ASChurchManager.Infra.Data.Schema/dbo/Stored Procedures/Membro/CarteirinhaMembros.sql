CREATE PROCEDURE CarteirinhaMembros
	@ID INT	
AS
BEGIN
	SELECT 
		M.Id
		, M.Nome
		, M.FotoPath AS Foto
		, M.FotoPath AS FotoByte
		, M.FotoUrl
		, M.NomePai
		, M.NomeMae 
		, M.NaturalidadeCidade 
		, M.NaturalidadeEstado
		, M.DataNascimento
		, M.EstadoCivil
		, M.RG
		, M.DataBatismoAguas
		, CM.DataCargo AS DataConsagracao
		, CM.LocalConsagracao
		, ISNULL(C.TipoCarteirinha, 0) AS TipoCarteirinha
		, M.DataValidadeCarteirinha
	FROM 
		Membro M 
		LEFT JOIN CargoMembro CM ON M.Id = CM.MembroId AND CM.DataCargo = (SELECT MAX(X.DataCargo) FROM CargoMembro X WHERE X.MembroId = M.Id) 
		LEFT JOIN Cargo C ON C.Id = CM.CargoId
	WHERE 
		M.Id = @ID
END