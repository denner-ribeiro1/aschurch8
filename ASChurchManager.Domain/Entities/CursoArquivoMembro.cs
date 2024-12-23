using ASChurchManager.Domain.Types;
using System;
using System.IO;

namespace ASChurchManager.Domain.Entities
{
    public class CursoArquivoMembro : BaseEntity
    {
        public TipoArquivoMembro TipoArquivo { get; set; }
        public int MembroId { get; set; }
        public string Descricao { get; set; }
        public int CursoId { get; set; }
        public string Local { get; set; }
        public string NomeCurso { get; set; }
        public DateTimeOffset? DataInicioCurso { get; set; }
        public DateTimeOffset? DataEncerramentoCurso { get; set; }
        public int CargaHoraria { get; set; }
        public string NomeArmazenado { get; set; }
        public string NomeOriginal { get; set; }
        public long Tamanho { get; set; }
        public string ContentType { get; set; }
        public Stream Arquivo { get; set; }
    }
}