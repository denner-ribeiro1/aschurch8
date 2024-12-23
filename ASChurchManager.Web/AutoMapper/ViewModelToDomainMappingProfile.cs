using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Admin.ViewModels.Cargo;
using ASChurchManager.Web.Areas.Admin.ViewModels.Curso;
using ASChurchManager.Web.Areas.Admin.ViewModels.Grupo;
using ASChurchManager.Web.Areas.Admin.ViewModels.TipoDeEvento;
using ASChurchManager.Web.Areas.Admin.ViewModels.Usuario;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Casamento;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregacao;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregado;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Eventos;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Nascimento;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante;
using ASChurchManager.Web.ViewModels.Shared;
using ASChurchManager.Web.ViewModels.Usuario;
using AutoMapper;
using System;

namespace ASChurchManager.Web.AutoMapperConfiguration
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CongregacaoVM, Congregacao>();
            CreateMap<CargoViewModel, Cargo>();
            CreateMap<TipoEventoViewModel, TipoEvento>();
            CreateMap<LoginViewModel, Usuario>();
            CreateMap<UsuarioViewModel, Usuario>();
            CreateMap<AlteracaoSenhaUsuarioViewModel, Usuario>();
            CreateMap<GridUsuarioViewModel, Usuario>();
            CreateMap<NascimentoViewModel, Nascimento>();
            CreateMap<CongregadoViewModel, Membro>();
            CreateMap<HistoricoCartasViewModel, HistoricoCartas>();
            CreateMap<GrupoViewModel, Grupo>();
            CreateMap<PastorCelebranteViewModel, Membro>();
            /***************************************************************************************************
             * ATENÇÃO: Manter os mapeamentos mais complexos abaixo para uma melhor legibilidade do código
             ***************************************************************************************************/
            CreateMap<ConfiguracaoBatismoViewModel, Batismo>()
                .ForMember(d => d.DataBatismo, o => o.MapFrom(s => s.DataBatismo.Add(((TimeSpan)s.HoraBatismo))));

            CreateMap<CartaViewModel, Carta>()
                .ForMember(d => d.TemplateId, o => o.MapFrom(s => s.TemplateCartaSelecionado));

            CreateMap<EnderecoViewModel, Endereco>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()));

            CreateMap<CasamentoViewModel, Casamento>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.CasamentoId))
                .ForMember(d => d.CongregacaoId, o => o.MapFrom(s => s.CongregacaoId))
                .ForMember(d => d.DataHoraInicio, o => o.MapFrom(s => ((DateTimeOffset)s.DataCasamento).Add(((TimeSpan)s.HoraInicio))))
                .ForMember(d => d.DataHoraFinal, o => o.MapFrom(s => ((DateTimeOffset)s.DataCasamento).Add(((TimeSpan)s.HoraFim))))
                .ForMember(d => d.NoivoId, o => o.MapFrom(s => s.NoivoMembro && s.NoivoId != null ? (long)s.NoivoId : 0))
                .ForMember(d => d.NoivoNome, o => o.MapFrom(s => !s.NoivoMembro ? s.NoivoNome : ""))
                .ForMember(d => d.PaiNoivoId, o => o.MapFrom(s => s.PaiNoivoMembro && s.PaiNoivoId != null ? (long)s.PaiNoivoId : 0))
                .ForMember(d => d.PaiNoivoNome, o => o.MapFrom(s => !s.PaiNoivoMembro ? s.PaiNoivoNome : ""))
                .ForMember(d => d.MaeNoivoId, o => o.MapFrom(s => s.MaeNoivoMembro && s.MaeNoivoId != null ? (long)s.MaeNoivoId : 0))
                .ForMember(d => d.MaeNoivoNome, o => o.MapFrom(s => !s.MaeNoivoMembro ? s.MaeNoivoNome : ""))
                .ForMember(d => d.NoivaId, o => o.MapFrom(s => s.NoivaMembro && s.NoivaId != null ? (long)s.NoivaId : 0))
                .ForMember(d => d.NoivaNome, o => o.MapFrom(s => !s.NoivaMembro ? s.NoivaNome : ""))
                .ForMember(d => d.PaiNoivaId, o => o.MapFrom(s => s.PaiNoivaMembro && s.PaiNoivaId != null ? (long)s.PaiNoivaId : 0))
                .ForMember(d => d.PaiNoivaNome, o => o.MapFrom(s => !s.PaiNoivaMembro ? s.PaiNoivaNome : ""))
                .ForMember(d => d.MaeNoivaId, o => o.MapFrom(s => s.MaeNoivaMembro && s.MaeNoivaId != null ? (long)s.MaeNoivaId : 0))
                .ForMember(d => d.MaeNoivaNome, o => o.MapFrom(s => !s.MaeNoivaMembro ? s.MaeNoivaNome : ""))
                .ForMember(d => d.PastorId, o => o.MapFrom(s => (!s.PastorMembro && s.PastorId != null ? (long)s.PastorId :
                                                                        s.PastorId != null && s.PastorId > 0 ? (long)s.PastorId : 0)))
                .ForMember(d => d.PastorNome, o => o.MapFrom(s => (!s.PastorMembro ? "" :
                                                                    s.PastorId != null && s.PastorId > 0 ? "" : s.PastorNome)));

            CreateMap<MembroViewModel, Membro>()
                .ForMember(d => d.FotoPath, o => o.MapFrom(s => s.Pessoa.FotoPath))
                .ForMember(d => d.FotoUrl, o => o.MapFrom(s => s.Pessoa.FotoUrl))
                .ForMember(d => d.Nome, o => o.MapFrom(s => s.Pessoa.Nome))
                .ForMember(d => d.RG, o => o.MapFrom(s => s.Pessoa.Rg))
                .ForMember(d => d.OrgaoEmissor, o => o.MapFrom(s => s.Pessoa.OrgaoEmissor))
                .ForMember(d => d.Cpf, o => o.MapFrom(s => s.Pessoa.Cpf))
                .ForMember(d => d.DataNascimento, o => o.MapFrom(s => s.Pessoa.DataNascimento))
                .ForMember(d => d.EstadoCivil, o => o.MapFrom(s => s.Pessoa.EstadoCivil))
                .ForMember(d => d.Sexo, o => o.MapFrom(s => s.Pessoa.Sexo))
                .ForMember(d => d.Escolaridade, o => o.MapFrom(s => s.Pessoa.Escolaridade))
                .ForMember(d => d.NomePai, o => o.MapFrom(s => s.Pessoa.NomePai))
                .ForMember(d => d.NomeMae, o => o.MapFrom(s => s.Pessoa.NomeMae))
                .ForMember(d => d.Nacionalidade, o => o.MapFrom(s => s.Pessoa.Nacionalidade))
                .ForMember(d => d.NaturalidadeCidade, o => o.MapFrom(s => s.Pessoa.NaturalidadeCidade))
                .ForMember(d => d.NaturalidadeEstado, o => o.MapFrom(s => s.Pessoa.NaturalidadeEstado != null ? s.Pessoa.NaturalidadeEstado.ToString() : ""))
                .ForMember(d => d.Profissao, o => o.MapFrom(s => s.Pessoa.Profissao))
                .ForMember(d => d.TelefoneCelular, o => o.MapFrom(s => s.Pessoa.TelefoneCelular))
                .ForMember(d => d.TelefoneComercial, o => o.MapFrom(s => s.Pessoa.TelefoneComercial))
                .ForMember(d => d.TelefoneResidencial, o => o.MapFrom(s => s.Pessoa.TelefoneResidencial))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Pessoa.Email))
                .ForMember(d => d.TituloEleitorNumero, o => o.MapFrom(s => s.Pessoa.TituloEleitorNumero))
                .ForMember(d => d.TituloEleitorSecao, o => o.MapFrom(s => s.Pessoa.TituloEleitorSecao))
                .ForMember(d => d.TituloEleitorZona, o => o.MapFrom(s => s.Pessoa.TituloEleitorZona))
                .ForMember(x => x.Situacoes, opt => opt.Ignore())
                .ForMember(x => x.Cargos, opt => opt.Ignore())
                .ForMember(x => x.Observacoes, opt => opt.Ignore());
            
            CreateMap<BatismoViewModel, Membro>()
                .ForMember(d => d.FotoPath, o => o.MapFrom(s => s.Pessoa.FotoPath))
                .ForMember(d => d.FotoUrl, o => o.MapFrom(s => s.Pessoa.FotoUrl))
                .ForMember(d => d.Nome, o => o.MapFrom(s => s.Pessoa.Nome))
                .ForMember(d => d.RG, o => o.MapFrom(s => s.Pessoa.Rg))
                .ForMember(d => d.OrgaoEmissor, o => o.MapFrom(s => s.Pessoa.OrgaoEmissor))
                .ForMember(d => d.Cpf, o => o.MapFrom(s => s.Pessoa.Cpf))
                .ForMember(d => d.DataNascimento, o => o.MapFrom(s => s.Pessoa.DataNascimento))
                .ForMember(d => d.EstadoCivil, o => o.MapFrom(s => s.Pessoa.EstadoCivil))
                .ForMember(d => d.NomePai, o => o.MapFrom(s => s.Pessoa.NomePai))
                .ForMember(d => d.NomeMae, o => o.MapFrom(s => s.Pessoa.NomeMae))
                .ForMember(d => d.Nacionalidade, o => o.MapFrom(s => s.Pessoa.Nacionalidade))
                .ForMember(d => d.NaturalidadeCidade, o => o.MapFrom(s => s.Pessoa.NaturalidadeCidade))
                .ForMember(d => d.NaturalidadeEstado, o => o.MapFrom(s => s.Pessoa.NaturalidadeEstado != null ? s.Pessoa.NaturalidadeEstado.ToString() : ""))
                .ForMember(d => d.Profissao, o => o.MapFrom(s => s.Pessoa.Profissao))
                .ForMember(d => d.TelefoneCelular, o => o.MapFrom(s => s.Pessoa.TelefoneCelular))
                .ForMember(d => d.TelefoneComercial, o => o.MapFrom(s => s.Pessoa.TelefoneComercial))
                .ForMember(d => d.TelefoneResidencial, o => o.MapFrom(s => s.Pessoa.TelefoneResidencial))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Pessoa.Email))
                .ForMember(d => d.TituloEleitorNumero, o => o.MapFrom(s => s.Pessoa.TituloEleitorNumero))
                .ForMember(d => d.TituloEleitorSecao, o => o.MapFrom(s => s.Pessoa.TituloEleitorSecao))
                .ForMember(d => d.TituloEleitorZona, o => o.MapFrom(s => s.Pessoa.TituloEleitorZona))
                .ForMember(d => d.Sexo, o => o.MapFrom(s => s.Pessoa.Sexo))
                .ForMember(d => d.Escolaridade, o => o.MapFrom(s => s.Pessoa.Escolaridade))
                .ForMember(x => x.Observacoes, opt => opt.Ignore());

            CreateMap<EventosViewModel, Evento>()
               .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
               .ForMember(d => d.CongregacaoId, o => o.MapFrom(s => s.CongregacaoId))
               .ForMember(d => d.TipoEventoId, o => o.MapFrom(s => s.TipoEventoSelecionado))
               .ForMember(d => d.Descricao, o => o.MapFrom(s => s.Descricao))
               .ForMember(d => d.DataHoraInicio, o => o.MapFrom(s => ((DateTimeOffset)s.DataInicio).Add(((TimeSpan)s.HoraInicio))));

            CreateMap<CursoVM, Curso>();
        }
    }
}