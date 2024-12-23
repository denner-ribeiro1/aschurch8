CREATE PROCEDURE dbo.ListarUsuarios
AS
BEGIN

	SET NOCOUNT ON

	SELECT
		U.Id
		, U.Nome
		, U.Username
		, U.Senha
		, U.Email
		, U.DataCriacao
		, U.DataAlteracao
		, U.AlterarSenhaProxLogin
		, CongregacaoId = C.Id
		, CongregacaoNome = C.Nome
		, CongregacaoSede = C.Sede
		, U.PerfilId
		, PerfilNome = P.Nome
		, PerfilTipo = P.TipoPerfil
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
	LEFT JOIN Perfil P ON U.PerfilId = P.Id
	WHERE
		U.Status <> 2
END