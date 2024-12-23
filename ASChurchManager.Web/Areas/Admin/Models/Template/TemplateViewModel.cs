using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Template
{
    public class TemplateViewModel : EntityViewModelBase
    {
        public TemplateViewModel()
        {
            ViewModelTitle = "Template";
            this.TagsDisponiveis = new List<TemplateTag>();
        }

        [Display(Name = "Nome"),
        Required(ErrorMessage = "Nome é obrigatório"),
        StringLength(100)]
        public string Nome { get; set; }

        [Display(Name = "Conteúdo"),
        Required(ErrorMessage = "Conteúdo é obrigatório")]
        public string Conteudo { get; set; }

        [Display(Name = "Tipo de Template"),
        Required(ErrorMessage = "Tipo de Template é obrigatório")]
        public TipoTemplate TipoTemplate { get; set; }

        [Display(Name = "Tags Disponíveis")]
        public List<TemplateTag> TagsDisponiveis { get; set; }

        [Display(Name = "Cabeçalho")]
        [Required(ErrorMessage = "Margem do Cabeçalho é obrigatório")]
        public int MargemAcima { get; set; }

        [Display(Name = "Rodapé")]
        [Required(ErrorMessage = "Margem do Rodapé é obrigatório")]
        public int MargemAbaixo { get; set; }

        [Display(Name = "Esquerda")]
        [Required(ErrorMessage = "Margem Esquerda é obrigatório")]
        public int MargemEsquerda { get; set; }

        [Display(Name = "Direita")]
        [Required(ErrorMessage = "Margem Direita é obrigatório")]
        public int MargemDireita { get; set; }
    }
}