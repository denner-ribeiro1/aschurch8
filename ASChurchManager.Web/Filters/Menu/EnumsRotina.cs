
namespace ASChurchManager.Web.Filters.Menu
{
    public enum Area
    {
        [Menu("Administração", "fa fa-bar-chart-o")]
        Admin,
        [Menu("Secretaria", "fa fa-book")]
        Secretaria
    }

    public enum Menu
    {
        [Menu("", "")]
        NaoDefinido,
        [Menu("Acesso", "fa fa-user")]
        Acesso,
        [Menu("Cadastros", "fa fa-pencil-square-o")]
        Cadastros,
        [Menu("Configurações", "fa fa-cog")]
        Configuracoes,
        [Menu("Membro", "")]
        Membro,
        [Menu("Batismo", "")]
        Batismo,
        [Menu("Casamento", "")]
        Casamento,
        [Menu("Congregação", "")]
        Congregacao,
        [Menu("Nascimento", "")]
        Nascimento,
        [Menu("Cargo", "")]
        Cargo,
        [Menu("Rotinas", "")]
        Rotina,
        [Menu("Tipo de Evento", "")]
        TipoEvento,
        [Menu("Template", "")]
        Template,
        [Menu("Usuário", "")]
        Usuario,
        [Menu("Congregado", "")]
        Congregado,
        [Menu("Cartas", "")]
        Carta,
        [Menu("Emissão/Recebimento", "")]
        EmissaoReceb,
        [Menu("Perfil", "")]
        Perfil,
        [Menu("Lista de Presença")]
        Chamada,
        [Menu("Cadastro")]
        Cadastro,
        [Menu("Eventos")]
        Eventos,
        [Menu("Relatórios")]
        Relatorios,
        [Menu("Candidatos ao Batismo")]
        CandidatosBatismo,
        [Menu("Aniversariantes")]
        Aniversariantes,
        [Menu("Nascimentos")]
        Nascimentos,
        [Menu("Transferências")]
        Transferencia,
        [Menu("Congregações")]
        Congregacoes,
        [Menu("Obreiros")]
        Obreiros,
        [Menu("Grupo", "")]
        Grupo,
        [Menu("Relatório Mensal")]
        RelMensal,
        [Menu("Configuração de Batismo")]
        ConfuguracaoBatismo,
        [Menu("Cadastro de Batismo")]
        CadastroBatismo,
        [Menu("Transferência sem Carta")]
        TransferenciaSemCarta,
        [Menu("Curso")]
        Cursos,
        [Menu("Membros")]
        RelMembros,
        [Menu("Cursos")]
        CursosMembro,
        [Menu("Casamentos")]
        Casamentos,
        [Menu("Eventos")]
        RelatorioEventos,
        [Menu("Controle de Presença")]
        Presenca,
        [Menu("Configuração")]
        ConfiguracaoPresenca,
        [Menu("Inscrição")]
        InscricaoPresenca,
        [Menu("Inscrição - Não Membros")]
        InscricaoNaoMembroPresenca,
        [Menu("Frequência Automática")]
        FrequenciaAutomatica,
        [Menu("Frequência Manual")]
        FrequenciaManual,
        [Menu("Emissão de Etiquetas")]
        EmissaoEtiquetas,
        [Menu("Presença - Frequência")]
        PresencaFrequencia,
        [Menu("Presença - Inscritos")]
        PresencaInscritos,
        [Menu("Inscrição - Captura")]
        InscricaoCapturaArquivoPresenca,
        [Menu("Presença - Planilha Frequência")]
        PresencaListaData,
        [Menu("Carteirinhas")]
        Carteirinhas
    }
}