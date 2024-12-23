CREATE PROCEDURE [dbo].[ConsultarMembroFotos]
AS
BEGIN
	SELECT
		Id,
		Nome,
		FotoPath
	FROM
		Membro
	Where 
		ISNULL(FotoPath, '') <> ''
END