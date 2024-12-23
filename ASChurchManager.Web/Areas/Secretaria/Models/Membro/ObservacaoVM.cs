using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class ObservacaoVM
    {
        public bool IsReadOnly { get; set; }
        [Display(Name = "Observação")]
        public string Observacao { get; set; }
        [Display(Name = "Data de Cadastro")]
        public string DataCadastroObs { get; set; }
        [Display(Name = "Responsável")]
        public string ResponsavelObs { get; set; }
        public int IdResponsavelObs { get; set; }
    }
}