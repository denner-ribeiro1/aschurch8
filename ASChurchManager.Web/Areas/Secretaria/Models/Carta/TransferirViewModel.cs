using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta
{
    public class TransferirViewModel
    {
        public TransferirViewModel()
        {
            Membros = new List<TransferirViewModel>();
        }

        [Display(Name = "Cod. Registro")]
        [Range(1, int.MaxValue, ErrorMessage = "Registro do Membro deve ser maior que zero")]
        public int? MembroId { get; set; }

        /// <summary>
        /// Nome membro
        /// </summary> 
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        /// <summary>
        /// Id da congregação de origem
        /// </summary>
        [Display(Name = "Código")]
        [Range(1, int.MaxValue, ErrorMessage = "Código da congregação de origem deve ser maior que zero")]
        public int? CongregacaoOrigemId { get; set; }

        /// <summary>
        /// Congregacao Origem
        /// </summary>
        [Display(Name = "Congregação")]
        public string CongregacaoOrigem { get; set; }

        /// <summary>
        /// Id congregacao destino
        /// </summary>
        [Display(Name = "Cód. Congregação")]
        [Range(1, int.MaxValue, ErrorMessage = "Código da congregação de destino deve ser maior que zero")]
        public int? CongregacaoDestId { get; set; }

        /// <summary>
        /// congregacao destino
        /// </summary>
        [Display(Name = "Congregação de Destino")]
        public string CongregacaoDest { get; set; }

        public IEnumerable<TransferirViewModel> Membros { get; set; }

        public string MembrosJSON
        {
            get
            {
                return JsonConvert.SerializeObject(Membros);
            }
            set
            {
                Membros = JsonConvert.DeserializeObject<List<TransferirViewModel>>(value);
            }
        }
    }
}