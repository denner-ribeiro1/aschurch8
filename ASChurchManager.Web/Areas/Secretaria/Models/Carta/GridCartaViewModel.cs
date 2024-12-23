using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta
{

    public class GridCartaItem : EntityViewModelBase
    {
        [Key]
        public override long Id { get; set; }

        [Display(Name = "Cod.Reg.")]
        public string MembroId { get; set; }

        [Display(Name = "Membro")]
        public string Nome { get; set; }

        [Display(Name = "Congr. Origem")]
        public string CongregacaoOrigem { get; set; }

        public long CongregacaoOrigemId { get; set; }

        [Display(Name = "Congr. Destino")]
        public string CongregacaoDestino { get; set; }

        public long CongregacaoDestinoId { get; set; }

        [Display(Name = "Tipo Carta")]
        public string TipoCarta { get; set; }

        [Display(Name = "Status")]
        public string StatusCarta { get; set; }

        public DateTimeOffset DataValidade { get; set; }

        public long TemplateId { get; set; }

        public bool AprovarCarta { get; set; }
    }
}