using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Grupo
{
    public class GridGrupoViewModel : EntityViewModelBase
    {
        public GridGrupoViewModel()
        {
            ViewModelTitle = "Grupos";
        }

        [Key]
        public override long Id { get; set; }
        [Display(Name = "Descrição Oficial do Grupo"),
        Required(ErrorMessage = "Descrição Oficial do Grupo é obrigatório"),
        StringLength(100)]
        public string Descricao { get; set; }
        [Display(Name = "Nome do Grupo"),
        Required(ErrorMessage = "Nome do Grupo é obrigatório"),
        StringLength(100)]
        public string NomeGrupo { get; set; }
        [Display(Name = "Nome do responsável pelo Grupo"),
        Required(ErrorMessage = "Nome do Responsável é obrigatório"),
        StringLength(100)]
        public string NomeResponsavel { get; set; }
        public long ResponsavelId { get; set; }
        public long GrupoId { get; set; }
    }
}