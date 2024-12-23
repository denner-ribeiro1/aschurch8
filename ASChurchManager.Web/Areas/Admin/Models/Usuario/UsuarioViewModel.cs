using ASChurchManager.Web.ViewModels.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Usuario
{
    public class UsuarioViewModel : EntityViewModelBase
    {
        public UsuarioViewModel()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "Usuário", Value = "Username" },
                new SelectListItem() { Text = "E-mail", Value = "Email" },
                new SelectListItem() { Text = "Congregação", Value = "CongregacaoId" }
            }, "Value", "Text");
        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }

        [Required]
        public string Nome { get; set; }

        [Display(Name = "E-mail"),
        DataType(DataType.EmailAddress, ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [StringLength(30, ErrorMessage = "O campo Usuário deve ter até 30 caracteres"),
         Display(Name = "Usuário"),
         Required(ErrorMessage = "O campo Usuário é obrigatório")]
        public string Username { get; set; }

        [Display(Name = "Senha"),
        Required(ErrorMessage = "O campo Senha é obrigatório"),
        DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password),
        Compare("Senha", ErrorMessage = "As senhas não são idênticas!"),
        Display(Name = "Redigite a Senha")]
        public string RedigiteSenha { get; set; }

        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
        Display(Name = "Cód. Congregação")]
        public int CongregacaoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
        Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        public bool AlterarSenhaProxLogin { get; set; }
        
        [Display(Name = "Perfil")]
        public IEnumerable<SelectListItem> SelectPerfil { get; set; }

        [Required(ErrorMessage = "O campo Perfil é obrigatório"),
        Display(Name = "Perfil")]
        public long PerfilId { get; set; }

        [Display(Name = "Permitir usuário Aprovar/Reprovar Membros.")]
        public bool PermiteAprovarMembro { get; set; }

        [Display(Name = "Permitir usuário Imprimir a Carteira de Membros.")]
        public bool PermiteImprimirCarteirinha { get; set; }

        [Display(Name = "Permitir usuário Atualizar/Excluir uma Situação do Membro.")]
        public bool PermiteExcluirSituacaoMembro { get; set; }

        [Display(Name = "Permitir usuário Atualizar/Excluir um Cargo do Membro.")]
        public bool PermiteExcluirCargoMembro { get; set; }

        [Display(Name = "Permitir usuário Atualizar/Excluir uma Observação do Membro.")]
        public bool PermiteExcluirObservacaoMembro { get; set; }

        [Display(Name = "Permitir usuário cadastrar 'Candidato ao Batismo' e 'Inscrições em Cursos/Eventos' após a Data Máxima permitida.")]
        public bool PermiteCadBatismoAposDataMaxima { get; set; }

        [Display(Name = "Permitir usuário Excluir Membros.")]
        public bool PermiteExcluirMembros { get; set; }

    }
}