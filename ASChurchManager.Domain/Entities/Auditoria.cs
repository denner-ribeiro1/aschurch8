using System;

namespace ASChurchManager.Domain.Entities
{
    public class Auditoria : BaseEntity
    {
        public long UsuarioId { get; set; }
        public string Controle { get; set; }
        public string Acao { get; set; }
        public string Ip { get; set; }
        public string Url { get; set; }
        public DateTimeOffset DataHora { get; set; }
        public string Parametros { get; set; }
        public string Navegador { get; set; }
    }
}
