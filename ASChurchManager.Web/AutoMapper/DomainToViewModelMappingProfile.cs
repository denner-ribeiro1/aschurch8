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
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {

            CreateMap<Congregacao, GridCongregacaoViewModel>();
            CreateMap<Cargo, CargoViewModel>();
            CreateMap<Cargo, GridCargosViewModel>();
            CreateMap<TipoEvento, TipoEventoViewModel>();
            CreateMap<TipoEvento, GridTipoEventoViewModel>();
            CreateMap<Nascimento, GridNascimentoViewModel>();
            CreateMap<Usuario, LoginViewModel>();
            CreateMap<Usuario, UsuarioViewModel>();
            CreateMap<Usuario, AlteracaoSenhaUsuarioViewModel>();
            CreateMap<Membro, CongregadoViewModel>();
            CreateMap<HistoricoCartas, HistoricoCartasViewModel>();
            CreateMap<HistoricoCartas, GridHistoricoCartasViewModel>();
            CreateMap<Grupo, GrupoViewModel>();
            CreateMap<Grupo, GridGrupoViewModel>();
            CreateMap<Membro, PastorCelebranteViewModel>();
            CreateMap<Membro, GridPastorCelebranteViewModel>();
            CreateMap<Batismo, GridConfiguracaoBatismoViewModel>();
            /***************************************************************************************************
             * ATENÇÃO: Manter os mapeamentos mais complexos abaixo para uma melhor legibilidade do código
             ***************************************************************************************************/
            CreateMap<Batismo, ConfiguracaoBatismoViewModel>()
                .ForMember(dest => dest.HoraBatismo, opt => opt.MapFrom(src => src.DataBatismo.ToString("H:mm")));

            CreateMap<Carta, CartaViewModel>()
                .ForMember(dest => dest.TemplateCartaSelecionado, opt => opt.MapFrom(src => src.TemplateId));

            CreateMap<Endereco, EnderecoViewModel>()
                .ForMember(x => x.BairroEstrangeiro, opt => opt.Ignore())
                .ForMember(x => x.CidadeEstrangeiro, opt => opt.Ignore())
                .ForMember(x => x.Provincia, opt => opt.Ignore())
                .ForMember(x => x.Estado, opt => opt.Ignore());

            CreateMap<Casamento, GridCasamentoViewModel>()
                .ForMember(dest => dest.CongregacaoNome, opt => opt.MapFrom(src => src.Congregacao.Nome))
                .ForMember(dest => dest.DataCasamento, opt => opt.MapFrom(src => src.DataHoraInicio))
                .ForMember(dest => dest.HoraInicio, opt => opt.MapFrom(src => src.DataHoraInicio.ToString("H:mm")))
                .ForMember(dest => dest.NoivoNome, opt => opt.MapFrom(src => src.NoivoNome))
                .ForMember(dest => dest.NoivaNome, opt => opt.MapFrom(src => src.NoivaNome));

            CreateMap<Usuario, GridUsuarioViewModel>()
                .ForMember(dest => dest.Congregacao, opt => opt.MapFrom(src => src.Congregacao.Nome));

            CreateMap<Membro, GridPresencaBatismoViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
               .ForMember(dest => dest.Congregacao, opt => opt.MapFrom(src => src.Congregacao.Nome))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != 0 ? "Presente" : ""));

            CreateMap<Membro, GridCongregadoViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Congregacao, opt => opt.MapFrom(src => src.Congregacao.Nome));

            CreateMap<Carta, GridCartaItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MembroId, opt => opt.MapFrom(src => src.MembroId))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.CongregacaoOrigem, opt => opt.MapFrom(src => src.CongregacaoOrigem))
                .ForMember(dest => dest.CongregacaoOrigemId, opt => opt.MapFrom(src => src.CongregacaoOrigemId))
                .ForMember(dest => dest.CongregacaoDestino, opt => opt.MapFrom(src => src.CongregacaoDest))
                .ForMember(dest => dest.CongregacaoDestinoId, opt => opt.MapFrom(src => src.CongregacaoDestId))
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
                .ForMember(dest => dest.TipoCarta, opt => opt.MapFrom(src => src.TipoCarta == TipoDeCarta.Transferencia ?
                                                                                              "Transferência" : src.TipoCarta == TipoDeCarta.Mudanca ?
                                                                                                     "Mudança" : "Recomendação"))
                .ForMember(dest => dest.StatusCarta, opt => opt.MapFrom(src => src.StatusCarta == StatusCarta.AguardandoRecebimento ?
                                                                                                "Aguardando Recebimento" : src.StatusCarta == StatusCarta.Cancelado ?
                                                                                                    "Cancelada" : "Finalizada"));
            CreateMap<Casamento, CasamentoViewModel>()
                .ForMember(d => d.CasamentoId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CongregacaoId, o => o.MapFrom(s => s.CongregacaoId))
                .ForMember(d => d.CongregacaoNome, o => o.MapFrom(s => s.Congregacao.Nome))
                .ForMember(d => d.PastorMembro, o => o.MapFrom(s => (s.Congregacao.CongregacaoResponsavelId == s.PastorId)))
                .ForMember(d => d.PastorId, o => o.MapFrom(s => s.PastorId))
                .ForMember(d => d.PastorNome, o => o.MapFrom(s => s.PastorNome))
                .ForMember(d => d.DataCasamento, o => o.MapFrom(s => s.DataHoraInicio))
                .ForMember(d => d.HoraInicio, o => o.MapFrom(s => s.DataHoraInicio.TimeOfDay))
                .ForMember(d => d.HoraFim, o => o.MapFrom(s => s.DataHoraFinal.TimeOfDay))
                .ForMember(d => d.NoivoMembro, o => o.MapFrom(s => (s.NoivoId > 0)))
                .ForMember(d => d.NoivoId, o => o.MapFrom(s => s.NoivoId))
                .ForMember(d => d.NoivoNome, o => o.MapFrom(s => s.NoivoNome))
                .ForMember(d => d.PaiNoivoMembro, o => o.MapFrom(s => (s.PaiNoivoId > 0)))
                .ForMember(d => d.PaiNoivoId, o => o.MapFrom(s => s.PaiNoivoId))
                .ForMember(d => d.PaiNoivoNome, o => o.MapFrom(s => s.PaiNoivoNome))
                .ForMember(d => d.MaeNoivoMembro, o => o.MapFrom((s => s.MaeNoivoId > 0)))
                .ForMember(d => d.MaeNoivoId, o => o.MapFrom(s => s.MaeNoivoId))
                .ForMember(d => d.MaeNoivoNome, o => o.MapFrom(s => s.MaeNoivoNome))
                .ForMember(d => d.NoivaMembro, o => o.MapFrom(s => (s.NoivaId > 0)))
                .ForMember(d => d.NoivaId, o => o.MapFrom(s => s.NoivaId))
                .ForMember(d => d.NoivaNome, o => o.MapFrom(s => s.NoivaNome))
                .ForMember(d => d.PaiNoivaMembro, o => o.MapFrom((s => s.PaiNoivaId > 0)))
                .ForMember(d => d.PaiNoivaId, o => o.MapFrom(s => s.PaiNoivaId))
                .ForMember(d => d.PaiNoivaNome, o => o.MapFrom(s => s.PaiNoivaNome))
                .ForMember(d => d.MaeNoivaMembro, o => o.MapFrom((s => s.MaeNoivaId > 0)))
                .ForMember(d => d.MaeNoivaId, o => o.MapFrom(s => s.MaeNoivaId))
                .ForMember(d => d.MaeNoivaNome, o => o.MapFrom(s => s.MaeNoivaNome));

            CreateMap<Membro, MembroViewModel>()
                .ForMember(x => x.Pessoa, opt => opt.MapFrom(d => new PessoaViewModel()
                {
                    FotoUrl = d.FotoUrl,
                    FotoPath = d.FotoPath,
                    Nome = d.Nome,
                    Rg = d.RG,
                    OrgaoEmissor = d.OrgaoEmissor,
                    Cpf = d.Cpf,
                    DataNascimento = d.DataNascimento,
                    EstadoCivil = d.EstadoCivil,
                    Escolaridade = d.Escolaridade,
                    Sexo = d.Sexo,
                    Nacionalidade = d.Nacionalidade,
                    NaturalidadeCidade = d.NaturalidadeCidade,
                    Profissao = d.Profissao,
                    TelefoneCelular = d.TelefoneCelular,
                    TelefoneComercial = d.TelefoneComercial,
                    TelefoneResidencial = d.TelefoneResidencial,
                    Email = d.Email,
                    TituloEleitorNumero = d.TituloEleitorNumero,
                    TituloEleitorSecao = d.TituloEleitorSecao,
                    TituloEleitorZona = d.TituloEleitorZona,
                }))
                .ForMember(x => x.Situacao, opt => opt.Ignore())
                .ForMember(x => x.Situacoes, opt => opt.Ignore())
                .ForMember(x => x.Cargo, opt => opt.Ignore())
                .ForMember(x => x.Cargos, opt => opt.Ignore())
                .ForMember(x => x.Observacao, opt => opt.Ignore())
                .ForMember(x => x.Observacoes, opt => opt.Ignore());

            CreateMap<Membro, BatismoViewModel>()
                .ForMember(x => x.Pessoa, opt => opt.MapFrom(d => new PessoaViewModel()
                {
                    FotoUrl = d.FotoUrl,
                    FotoPath = d.FotoPath,
                    Nome = d.Nome,
                    Rg = d.RG,
                    Cpf = d.Cpf,
                    OrgaoEmissor = d.OrgaoEmissor,
                    DataNascimento = d.DataNascimento,
                    EstadoCivil = d.EstadoCivil,
                    Nacionalidade = d.Nacionalidade,
                    NaturalidadeCidade = d.NaturalidadeCidade,
                    Profissao = d.Profissao,
                    TelefoneCelular = d.TelefoneCelular,
                    TelefoneComercial = d.TelefoneComercial,
                    TelefoneResidencial = d.TelefoneResidencial,
                    Email = d.Email,
                    TituloEleitorNumero = d.TituloEleitorNumero,
                    TituloEleitorSecao = d.TituloEleitorSecao,
                    TituloEleitorZona = d.TituloEleitorZona,
                    Sexo = d.Sexo,
                    Escolaridade = d.Escolaridade,
                    NaturalidadeEstado = (!string.IsNullOrWhiteSpace(d.NaturalidadeEstado) &&
                                           Enum.IsDefined(typeof(Estado), d.NaturalidadeEstado) ? (Estado)Enum.Parse(typeof(Estado), d.NaturalidadeEstado) : Estado.SP)

                }))
                .ForMember(x => x.Observacao, opt => opt.Ignore())
                .ForMember(x => x.Observacoes, opt => opt.Ignore());

            CreateMap<Nascimento, NascimentoViewModel>()
                .ForMember(d => d.MaeMembro, o => o.MapFrom(s => (s.IdMembroMae > 0)))
                .ForMember(d => d.PaiMembro, o => o.MapFrom(s => (s.IdMembroPai > 0)))
                .ForMember(d => d.PastorMembro, o => o.MapFrom(s => (s.congregacao.CongregacaoResponsavelId == s.PastorId ? true : false)));

            CreateMap<Evento, EventosViewModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TipoEventoSelecionado, o => o.MapFrom(s => s.TipoEventoId))
                .ForMember(d => d.CongregacaoId, o => o.MapFrom(s => s.CongregacaoId))
                .ForMember(d => d.CongregacaoNome, o => o.MapFrom(s => s.Congregacao.Nome))
                .ForMember(d => d.DataInicio, o => o.MapFrom(s => s.DataHoraInicio.Date))
                .ForMember(d => d.HoraInicio, o => o.MapFrom(s => s.DataHoraInicio.TimeOfDay))
                .ForMember(d => d.HoraFinal, o => o.MapFrom(s => s.DataHoraFim.TimeOfDay));

            CreateMap<Curso, CursoVM>();

        }
    }
}
