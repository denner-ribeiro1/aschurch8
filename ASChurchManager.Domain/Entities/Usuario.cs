namespace ASChurchManager.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public Usuario()
        {
            Congregacao = new Congregacao();
            Perfil = new Perfil();
        }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string CongregacaoNome { get; set; }
        public string Senha { get; set; }
        public Congregacao Congregacao { get; set; }
        public bool AlterarSenhaProxLogin { get; set; }
        public Perfil Perfil { get; set; }
        public bool PermiteAprovarMembro { get; set; }
        public bool PermiteImprimirCarteirinha { get; set; }
        public bool PermiteExcluirSituacaoMembro { get; set; }
        public bool PermiteCadBatismoAposDataMaxima { get; set; }
        public bool PermiteExcluirMembros { get; set; }
        public bool PermiteExcluirCargoMembro { get; set; }
        public bool PermiteExcluirObservacaoMembro { get; set; }
        public string SignalRConnectionId { get; set; }
        public string Skin { get; set; }
    }
}