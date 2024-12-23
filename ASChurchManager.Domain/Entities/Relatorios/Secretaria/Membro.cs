namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Membro
    {
        public long Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Congregacao { get; set; }

        public string DataValidadeCarteirinha { get; set; }

        public string Cargo { get; set; }
    }
}
