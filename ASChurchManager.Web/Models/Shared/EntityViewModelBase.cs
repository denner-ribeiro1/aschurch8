using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.ViewModels.Shared
{
    public abstract class EntityViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Id do registro
        /// Obs: Deve ser sobrescrito caso seja utilizado no Grid.Mvc para ser a primeira coluna
        /// </summary>
        [Key]
        public virtual long Id { get; set; }

        /// <summary>
        /// Data de Criação do registro
        /// </summary>
        [Display(Name = "Data Criação"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public virtual DateTimeOffset DataCriacao { get; set; }

        /// <summary>
        /// Data de alteração do registro
        /// </summary>
        [Display(Name = "Data Alteração"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public virtual DateTimeOffset DataAlteracao { get; set; }
    }
}