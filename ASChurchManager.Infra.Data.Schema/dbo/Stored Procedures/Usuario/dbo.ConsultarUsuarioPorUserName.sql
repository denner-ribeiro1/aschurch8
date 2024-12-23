CREATE PROCEDURE ConsultarUsuarioPorUserName
	@UserName VARCHAR(30)
AS
BEGIN
	SELECT 
		U.Id
		, U.Nome
		, U.Username
		, U.Email
		, U.AlterarSenhaProxLogin
		, U.DataCriacao
		, U.DataAlteracao
		, U.CongregacaoId
		, C.Nome AS CongregacaoNome
		, C.Sede AS CongregacaoSede
		, C.Id AS CongregacaoId
		, U.PermiteAprovarMembro
		, U.PermiteImprimirCarteirinha
		, U.PermiteExcluirSituacaoMembro
		, U.PermiteCadBatismoAposDataMaxima
		, U.PermiteExcluirMembros
		, U.PermiteExcluirCargoMembro
		, U.PermiteExcluirObservacaoMembro
		, U.Skin
	FROM Usuario U
	LEFT JOIN Congregacao C ON U.CongregacaoId = C.Id
	WHERE
		U.Username = @UserName
END