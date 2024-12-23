CREATE PROCEDURE ValidarLogin
	@Username VARCHAR(30),
	@Senha VARCHAR(1000)
As
BEGIN

	SET NOCOUNT ON

	SELECT 
		U.Id
		, U.Nome
		, U.Username
		, U.Email
		, U.AlterarSenhaProxLogin
		, U.DataCriacao
		, U.DataAlteracao
		, U.CongregacaoId
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
		, ISNULL(U.Skin, 'skin-blue') AS Skin 
	From Usuario U
	LEFT JOIN Congregacao C ON U.CongregacaoId = C.Id
	LEFT JOIN Perfil P ON U.PerfilId = P.Id
	WHERE 
		Username = @Username 
		AND Senha = @Senha
		AND U.Status = 1
END