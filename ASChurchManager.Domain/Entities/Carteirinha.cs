using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Domain.Entities
{
    public class Carteirinha
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string FotoPath { get; set; }
        public string FotoUrl { get; set; }
        public byte[] FotoByte { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string DataNascimento { get; set; }
        public string EstadoCivil { get; set; }
        public string RG { get; set; }
        public string DataBatismoAguas { get; set; }
        public string DataValidadeCarteirinha { get; set; }
        public string DataConsagracao { get; set; }
        public string LocalConsagracao { get; set; }
        public TipoCarteirinha TipoCarteirinha { get; set; }
    }
}
