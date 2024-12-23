using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASChurchManager.Application.AppServices
{
    public class CursoArquivoMembroAppService : BaseAppService<CursoArquivoMembro>, ICursoArquivoMembroAppService
    {
        #region Variaveis
        private readonly ICursoArquivoMembroRepository _cursoService;
        private readonly IArquivosAzureAppService arqAzureService;
        #endregion

        #region Construtor
        public CursoArquivoMembroAppService(ICursoArquivoMembroRepository cursoService,
            IArquivosAzureAppService arquivosAzureAppService)
            : base(cursoService)
        {
            _cursoService = cursoService;
            arqAzureService = arquivosAzureAppService;
        }
        #endregion

        public void BeginTran()
        {
            _cursoService.BeginTran();
        }

        public void Commit()
        {
            _cursoService.Commit();
        }

        public async Task<string> DeleteFileAsync(CursoArquivoMembro arquivoMembro)
        {
            try
            {
                var arq = _cursoService.GetById(arquivoMembro.Id, 0);

                if (arq != null && arq.Id > 0)
                {
                    _cursoService.BeginTran();
                    _cursoService.Delete(arq);
                    if (!string.IsNullOrWhiteSpace(arq.NomeArmazenado))
                    {
                        var container = $"arquivo{arq.MembroId}";
                        if (arq.TipoArquivo == Domain.Types.TipoArquivoMembro.Curso)
                            container = $"curso{arq.MembroId}";
                        await arqAzureService.DeleteSpecificFileAsync(arq.NomeArmazenado, container);
                    }
                    _cursoService.Commit();
                    return arq.NomeOriginal;
                }
                else
                    throw new Exception("Arquivo não encontrado para a exclusão.");
            }
            catch (Exception ex)
            {
                _cursoService.RollBack();
                throw new Exception($"Falha ao excluir o Arquivo: {ex.Message}");
            }
        }

        public async Task<RetornoAzure> DownloadFileAsync(CursoArquivoMembro arquivoMembro)
        {
            arquivoMembro = GetById(arquivoMembro.Id, 0);

            var container = $"arquivo{arquivoMembro.MembroId}";
            if (arquivoMembro.TipoArquivo == Domain.Types.TipoArquivoMembro.Curso)
                container = $"curso{arquivoMembro.MembroId.ToString()}";
            var arquivo = await arqAzureService.DownloadFromStorageAsync(arquivoMembro.NomeArmazenado, container);
            return arquivo;
        }

        public IEnumerable<CursoArquivoMembro> GetArquivoByMembro(long membroId)
        {
            return _cursoService.GetArquivoByMembro(membroId);
        }

        public void RollBack()
        {
            _cursoService.RollBack();
        }

        public bool UploadFile(CursoArquivoMembro arquivoMembro)
        {
            try
            {
                _cursoService.BeginTran();
                arquivoMembro.Id = _cursoService.Add(arquivoMembro);
                if (arquivoMembro.Id > 0)
                {

                    if (arquivoMembro.Arquivo != null)
                    {
                        arquivoMembro.NomeArmazenado = $"{arquivoMembro.Id}_{arquivoMembro.MembroId}_{arquivoMembro.NomeOriginal}";
                        var container = $"arquivo{arquivoMembro.MembroId.ToString()}";
                        if (arquivoMembro.TipoArquivo == Domain.Types.TipoArquivoMembro.Curso)
                            container = $"curso{arquivoMembro.MembroId.ToString()}";

                        arqAzureService.UploadFileToStorage(arquivoMembro.Arquivo, arquivoMembro.NomeArmazenado, container, arquivoMembro.ContentType);
                        _cursoService.Update(arquivoMembro);
                    }
                    _cursoService.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                _cursoService.RollBack();
                throw new Exception($"Falha ao Adicionar o Arquivo: {ex.Message}");
            }
        }
    }
}