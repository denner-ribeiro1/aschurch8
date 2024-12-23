CREATE PROCEDURE ConsultarUsuario
	@Id INT
AS
BEGIN
	
	Select 
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
		, U.Skin
	From Usuario U
	LEFT JOIN Congregacao C ON U.CongregacaoId = C.Id
	LEFT JOIN Perfil P ON U.PerfilId = P.Id
	WHERE
		U.Id = @Id
		And U.Status <> 2

END