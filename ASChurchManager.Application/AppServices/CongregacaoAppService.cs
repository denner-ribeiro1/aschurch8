using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Entities.Relatorios.API.In;
using ASChurchManager.Domain.Entities.Relatorios.API.Out;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.WebApi.Oauth.Client;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ASChurchManager.Application.AppServices
{
    public sealed class CongregacaoAppService : BaseAppService<Congregacao>, ICongregacaoAppService
    {
        #region Variaveis
        private readonly ICongregacaoRepository _congregacaoService;
        private readonly IClientAPIAppServices _clientService;
        private readonly IArquivosAzureAppService _arqAzureService;
        #endregion

        #region Construtor
        public CongregacaoAppService(ICongregacaoRepository congregacaoService,
            IClientAPIAppServices clientService
            , IArquivosAzureAppService arquivosAzureAppService) :
            base(congregacaoService)
        {
            _congregacaoService = congregacaoService;
            _clientService = clientService;
            _arqAzureService = arquivosAzureAppService;
        }
        #endregion

        #region Publicos
        public short DefinirSede(long id)
        {
            return _congregacaoService.DefinirSede(id);
        }

        public bool ExisteSede()
        {
            var congregacaoSede = this.GetSede();
            return congregacaoSede != null;
        }

        public IEnumerable<Congregacao> GetAll(bool completo = true)
        {
            return _congregacaoService.GetAll(completo);
        }

        public Congregacao GetById(long id)
        {
            return _congregacaoService.GetById(id);
        }

        public Congregacao GetSede()
        {
            var congregacaoSede = _congregacaoService.GetSede();
            return congregacaoSede;
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            return Delete(id, 0, usuarioID);
        }
        public override int Delete(Congregacao entity, long usuarioID = 0)
        {
            return Delete(entity.Id, usuarioID);
        }

        public int Delete(long id, long congregacaoId, long usuarioID)
        {
            var ret = _congregacaoService.Delete(id, congregacaoId, usuarioID);
            var container = $"congr{id}";
            _arqAzureService.DeleteSpecificContainerAsync(container).Wait();
            return ret;
        }

        public IEnumerable<CongregacaoObreiro> ListarObreirosCongregacaoPorMembroId(long membroId)
        {
            return _congregacaoService.ListarObreirosCongregacaoPorMembroId(membroId);
        }

        public FichaCongregacao RelatorioFichaCongregacao(long id)
        {
            return _congregacaoService.RelatorioFichaCongregacao(id);
        }

        public bool FichaCongregacao(int id, int usuarioId, out byte[] relatorio, out string mimeType)
        {
            var param = new InFichaCongregacao()
            {
                CongregacaoId = id
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/FichaCongregacao", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar a Ficha de Congregação. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public IEnumerable<Congregacao> BuscarCongregacao(int pageSize, int rowStart, string sorting, out int rowCount, string campo, string valor, long usuarioId)
        {
            return _congregacaoService.BuscarCongregacao(pageSize, rowStart, sorting, out rowCount, campo, valor, usuarioId);
        }

        public IEnumerable<Congregacao> ListarCongregacaoPaginado(int pageSize, int rowStart, string sorting, string filtro, string conteudo, long usuarioID, out int rowCount)
        {
            return _congregacaoService.ListarCongregacaoPaginado(pageSize, rowStart, sorting, filtro, conteudo, usuarioID, out rowCount);
        }

        public IEnumerable<CongregacaoGrupo> ListarGrupoCongregacao(long congregacaoId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CongregacaoObreiro> ListarObreirosCongregacao(long congregacaoId)
        {
            return _congregacaoService.ListarObreirosCongregacao(congregacaoId);
        }

        public IEnumerable<ObservacaoCongregacao> ListarObservacaoCongregacao(long congregacaoId)
        {
            return _congregacaoService.ListarObservacaoCongregacao(congregacaoId);
        }

        public List<ArquivoAzure> ListaArquivos(int id)
        {
            var container = $"congr{id}";
            var lista = _arqAzureService.ListaArquivos(container);
            return lista;
        }

        public Task<RetornoAzure> DownloadArquivo(int id, string nomeArquivo)
        {
            var container = $"congr{id}";
            return _arqAzureService.DownloadFromStorageAsync(nomeArquivo, container);
        }

        public Task<bool> DeleteFilesAsync(int id, string nomeArquivo)
        {
            var container = $"congr{id}";
            return _arqAzureService.DeleteSpecificFileAsync(nomeArquivo, container);
        }

        public string UploadFileToStorage(int id, IFormFile file)
        {
            var container = $"congr{id}";
            var nome = file.FileName.Trim('\"');
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            return _arqAzureService.UploadArrayByteAsync(fileBytes, nome, container).Result;
        }

        public IEnumerable<QuantidadePorCongregacao> ConsultarQtdMembrosCongregacao(long congregacaoId)
        {
            return _congregacaoService.ConsultarQtdMembrosCongregacao(congregacaoId);
        }

        
        #endregion
    }
}