using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IMembroRepository : IRepositoryDAO<Membro>
    {
        void BeginTran();
        void Commit();
        void RollBack();
        bool ExisteCodigoRegistro(long codigoRegistro);
        void AdicionarSituacao(SituacaoMembro situacao);
        void ExcluirSituacao(long membroId, long situacaoId);
        IEnumerable<SituacaoMembro> ListarSituacoesMembro(long membroId);
        void AdicionarCargo(long membroId, long cargoId, string LocalConsagracao, DateTimeOffset dataCargo);
        void ExcluirCargo(long membroId, long cargoId);
        IEnumerable<CargoMembro> ListarCargosMembro(long membroId);
        Membro ExisteCPFDuplicado(long membroId, string cpf);
        IEnumerable<ObservacaoMembro> ListarObservacaoMembro(long membroId);
        void AdicionarObservacao(ObservacaoMembro obsMembro);
        void ExcluirObservacao(long id);
        void AprovarReprovaMembro(long id, long usuarioId, Types.Status status, string motivoReprovacao);
        IEnumerable<HistoricoCartas> ListarHistoricoCartas(long membroId);
        IEnumerable<Membro> ListarMembrosPendencias(int pageSize, int rowStart, out int rowCount, string sorting, long usuarioID);
        IEnumerable<Carteirinha> CarteirinhaMembros(long membroId);
        IEnumerable<Carteirinha> ListarCarteirinhaMembros(int[] membroId);
        void AtualizarValidadeCarteirinha(long membroId);
        RelatorioFichaMembro FichaMembro(long membroId);
        IEnumerable<Membro> BuscarMembros(int pageSize, int rowStart, out int rowCount, string campo, string valor);
        IEnumerable<Membro> ListarMembroPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID,
            TipoMembro tipoMembro = TipoMembro.NaoDefinido, Types.Status status = Types.Status.NaoDefinido);
        Membro GetByCPF(string cpf, bool completo = false);
        long AtualizarMembroExterno(Membro membro, string ip);
        Dictionary<string, Membro> GetMembroConfirmado(long membroId);
        IEnumerable<Membro> ListarMembroObreiroPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, int congregacaoId, long usuarioID);
        long RestaurarMembroConfirmado(long membroId, string campos, long usuarioId);
        void AtualizarMembroFotoUrl(long id, string fotoUrl);
        void AtualizarSenha(long Id, string SenhaAtual, string NovaSenha, bool atualizarSenha);

    }
}