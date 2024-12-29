using ASBaseLib.Security.Cryptography.Providers;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Entities.Relatorios.API.In;
using ASChurchManager.Domain.Entities.Relatorios.API.Out;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Interfaces.Repository;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.WebApi.Oauth.Client;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace ASChurchManager.Application.AppServices
{
    public class MembroAppService : BaseAppService<Membro>, IMembroAppService
    {
        private readonly IMembroRepository _membroRepository;
        private readonly IClientAPIAppServices _clientService;
        private readonly IConfiguration _configuration;
        private readonly IPaisRepository _paisRepository;
        private readonly IEmailAppService _emailAppService;

        private StorageCredentials storageCredentials;

        private CloudStorageAccount storageAccount;

        public MembroAppService(IMembroRepository membroService,
            IClientAPIAppServices clientService,
            IConfiguration configuration
            , IPaisRepository paisRepository,
            IEmailAppService emailAppService) :
            base(membroService)
        {
            _membroRepository = membroService;
            _clientService = clientService;
            _configuration = configuration;
            _emailAppService = emailAppService;

            var accountName = _configuration["AzureStorage:AccountName"];
            var accountKey = _configuration["AzureStorage:AccountKey"];

            storageCredentials = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(storageCredentials, true);

            _paisRepository = paisRepository;
        }

        public bool ExisteCodigoRegistro(long codigoRegistro)
        {
            return _membroRepository.ExisteCodigoRegistro(codigoRegistro);
        }

        public void AdicionarSituacao(SituacaoMembro situacao)
        {
            _membroRepository.AdicionarSituacao(situacao);
        }

        public IEnumerable<SituacaoMembro> ListarSituacoesMembro(long membroId)
        {
            return _membroRepository.ListarSituacoesMembro(membroId);
        }

        public void ExcluirSituacao(long membroId, long situacaoId)
        {
            _membroRepository.ExcluirSituacao(membroId, situacaoId);
        }

        public void AdicionarCargo(long membroId, long cargoId, string LocalConsagracao, DateTimeOffset dataCargo)
        {
            _membroRepository.AdicionarCargo(membroId, cargoId, LocalConsagracao, dataCargo);
        }

        public void ExcluirCargo(long membroId, long cargoId)
        {
            _membroRepository.ExcluirCargo(membroId, cargoId);
        }

        public IEnumerable<CargoMembro> ListarCargosMembro(long membroId)
        {
            return _membroRepository.ListarCargosMembro(membroId);
        }

        public Membro ExisteCPFDuplicado(long membroId, string cpf)
        {
            return _membroRepository.ExisteCPFDuplicado(membroId, cpf);
        }

        public IEnumerable<ObservacaoMembro> ListarObservacaoMembro(long membroId)
        {
            return _membroRepository.ListarObservacaoMembro(membroId);
        }

        public void AdicionarObservacao(ObservacaoMembro obsMembro)
        {
            _membroRepository.AdicionarObservacao(obsMembro);
        }

        public void ExcluirObservacao(long id)
        {
            _membroRepository.ExcluirObservacao(id);
        }

        public void AprovarReprovaMembro(long membroId, long usuarioId, Status status, string motivoReprovacao)
        {
            _membroRepository.AprovarReprovaMembro(membroId, usuarioId, status, motivoReprovacao);
        }

        public IEnumerable<HistoricoCartas> ListarHistoricoCartas(long membroId)
        {
            return _membroRepository.ListarHistoricoCartas(membroId);
        }

        public IEnumerable<Membro> ListarMembrosPendencias(int pageSize, int rowStart, out int rowCount, string sorting, long usuarioID)
        {
            return _membroRepository.ListarMembrosPendencias(pageSize, rowStart, out rowCount, sorting, usuarioID);
        }

        public IEnumerable<Carteirinha> CarteirinhaMembro(long membroId)
        {
            return _membroRepository.CarteirinhaMembros(membroId);
        }

        public void AtualizarValidadeCarteirinha(long membroId)
        {
            _membroRepository.AtualizarValidadeCarteirinha(membroId);
        }

        public RelatorioFichaMembro FichaMembro(long membroId)
        {
            return _membroRepository.FichaMembro(membroId);
        }

        public async Task DeleteAndDeleteFilesAsync(long id)
        {
            try
            {
                _membroRepository.BeginTran();

                var membro = GetById(id, 0);
                _membroRepository.Delete(id);

                if (!string.IsNullOrWhiteSpace(membro.FotoUrl))
                {
                    var blob = new CloudBlockBlob(new Uri(membro.FotoUrl), storageAccount.Credentials);
                    blob.DeleteIfExists();
                }

                var container = $"arquivo{id.ToString()}";
                await DeleteSpecificContainerAsync(container);

                container = $"curso{id.ToString()}";
                await DeleteSpecificContainerAsync(container);


                _membroRepository.Commit();
            }
            catch (Exception ex)
            {
                _membroRepository.RollBack();
                throw new Exception($"Falha ao excluir o Arquivo: {ex.Message}");
            }
        }

        public Carteirinha CarteirinhaMembro(int membroId, bool atualizarValidade)
        {
            var cart = CarteirinhaMembro(membroId);
            if (!cart.Any())
                throw new Exception("Membro não encontrado.");

            /*Atualiza a data de Validade da carteirinha*/
            if ((cart.FirstOrDefault().TipoCarteirinha != TipoCarteirinha.Membro &&
                string.IsNullOrWhiteSpace(cart.FirstOrDefault().DataValidadeCarteirinha)) || atualizarValidade)
            {
                AtualizarValidadeCarteirinha(membroId);
                cart = CarteirinhaMembro(membroId);
            }
            return cart.FirstOrDefault();
        }

        public bool FichaMembro(int id, int usuarioId, out byte[] relatorio, out string mimeType)
        {
            var param = new InFichaMembro()
            {
                MembroId = id
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/FichaMembro", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar a Ficha de Membro. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public IEnumerable<Membro> BuscarMembros(int pageSize, int rowStart, out int rowCount, string campo, string valor)
        {
            return _membroRepository.BuscarMembros(pageSize, rowStart, out rowCount, campo, valor);
        }

        public IEnumerable<Membro> ListarMembroPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID,
            TipoMembro tipoMembro = TipoMembro.NaoDefinido, Status status = Status.NaoDefinido)
        {
            return _membroRepository.ListarMembroPaginado(pageSize, rowStart, out rowCount, sorting, campo, valor, usuarioID, tipoMembro, status);
        }

        public void BeginTran()
        {
            _membroRepository.BeginTran();
        }

        public void Commit()
        {
            _membroRepository.Commit();
        }

        public void RollBack()
        {
            _membroRepository.RollBack();
        }

        public IEnumerable<Carteirinha> CarteirinhaMembros(long membroId)
        {
            return _membroRepository.CarteirinhaMembros(membroId);
        }

        public bool DeleteSpecificFile(string fileName, string containerName)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.DeleteIfExistsAsync();
            return true;
        }

        public byte[] DownloadFromStorage(string fileName, string containerName, out string contentType)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                var blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.FetchAttributesAsync();
                long fileByteLength = blockBlob.Properties.Length;
                contentType = blockBlob.Properties.ContentType;

                byte[] fileContent = new byte[fileByteLength];
                blockBlob.DownloadToByteArrayAsync(fileContent, 0);

                return fileContent;
            }
            catch
            {
                contentType = "";
                return null;
            }
        }

        public bool UploadFileToStorage(Stream fileStream, string fileName, string containerName)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync();
            var blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromStreamAsync(fileStream);
            return true;
        }

        public async Task<bool> DeleteSpecificContainerAsync(string containerName)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blobExists = await container.ExistsAsync();

            if (blobExists)
            {
                await container.FetchAttributesAsync();
                await container.DeleteIfExistsAsync();
            }

            return true;
        }

        public override long Add(Membro entity, long usuarioID = 0)
        {
            var id = base.Add(entity, usuarioID);
            entity.Id = id;
            return id;
        }

        public Membro GetByCPF(string cpf, bool completo = false)
        {
            return _membroRepository.GetByCPF(cpf, completo);
        }

        public long AtualizarMembroExterno(Membro membro, string ip)
        {
            return _membroRepository.AtualizarMembroExterno(membro, ip);
        }

        public Dictionary<string, Membro> GetMembroConfirmado(long membroId)
        {
            return _membroRepository.GetMembroConfirmado(membroId);
        }

        public IEnumerable<Membro> ListarMembroObreiroPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, int congregacaoId, long usuarioID)
        {
            return _membroRepository.ListarMembroObreiroPaginado(pageSize, rowStart, out rowCount, sorting, campo, valor, congregacaoId, usuarioID);
        }

        public long RestaurarMembroConfirmado(long membroId, string campos, long usuarioId)
        {
            return _membroRepository.RestaurarMembroConfirmado(membroId, campos, usuarioId);
        }

        public void AtualizarMembroFotoUrl(long id, string fotoUrl)
        {
            _membroRepository.AtualizarMembroFotoUrl(id, fotoUrl);
        }

        public IEnumerable<Pais> ConsultarPaises()
        {
            return _paisRepository.GetAll(0);
        }

        public IEnumerable<Carteirinha> ListarCarteirinhaMembros(int[] membroId)
        {
            return _membroRepository.ListarCarteirinhaMembros(membroId);
        }

        public (bool, Membro) ValidarLogin(string cpf, string senha)
        {
            var membro = _membroRepository.GetByCPF(cpf, false);
            var Senha = Hash.GetHash(senha, CryptoProviders.HashProvider.MD5);

            return (Senha == membro.Senha, membro);
        }

        public bool ValidarSenha(int id, string senhaAtual)
        {
            var membro = _membroRepository.GetById(id, 0);
            if (membro.Id == id)
            {
                var senha = Hash.GetHash(senhaAtual, CryptoProviders.HashProvider.MD5);
                return senha == membro.Senha;
            }
            else
                return false;
        }

        public void AtualizarSenha(long Id, string SenhaAtual, string NovaSenha, bool atualizarSenha)
        {
            var membro = _membroRepository.GetById(Id, 0);
            if (membro.Id == Id)
            {
                var senha = Hash.GetHash(SenhaAtual, CryptoProviders.HashProvider.MD5);
                if (senha == membro.Senha)
                {
                    var senhaNova = Hash.GetHash(NovaSenha, CryptoProviders.HashProvider.MD5);
                    _membroRepository.AtualizarSenha(Id, SenhaAtual, senhaNova, atualizarSenha);
                }
                else
                    throw new Erro("Senha atual incorreta");
            }
            else
                throw new Erro("Membro não encontrado");
        }

        public (bool, string) InscricaoApp(string cpf, string nomeMae, DateTime dataNascimento, string email)
        {
            var membro = _membroRepository.GetByCPF(cpf, false);

            if (membro != null && membro.Id == 0)
                return (false, "CPF não localizado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro");

            if (membro.Status != Status.Ativo)
                return (false, "Membro não localizado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");

            if (string.IsNullOrEmpty(membro.Email) && string.IsNullOrEmpty(email))
                return (false, "E-mail não localizado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");

            if (string.IsNullOrEmpty(email))
                email = membro.Email;

            if (!string.IsNullOrWhiteSpace(membro.NomeMae))
            {
                var nomeMaeBD = membro.NomeMae.Trim().Split(' ');
                if (nomeMaeBD.Length > 0)
                {
                    if (string.IsNullOrEmpty(nomeMaeBD[0]) || nomeMaeBD[0].Trim().ToUpper() != nomeMae.Trim().ToUpper())
                        return (false, "O Nome da Mãe não corresponde ao Cadastro! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");
                }
                else
                    return (false, "O Nome da Mãe não corresponde ao Cadastro!Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");
            }
            else
                return (false, "O Nome da Mãe não corresponde ao Cadastro! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");


            if (membro.DataNascimento.Value.Date != dataNascimento.Date)
                return (false, "Data de Nascimento! <br />Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");

            var novaSenha = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6).ToUpper();
            var senhaCriptografada = Hash.GetHash(novaSenha, CryptoProviders.HashProvider.MD5);

            _membroRepository.AtualizarSenha(membro.Id, "", senhaCriptografada, true);

            var conteudo = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Infra", "Emails", "email_inscricao.txt"));
            conteudo = conteudo.Replace("[NovaSenha]", novaSenha);

            _emailAppService.EnviarEmail(email.ToLower(), "Inscrição - Senha de acesso", conteudo);

            if (membro.Email.ToLower() != email.ToLower())
                _membroRepository.AtualizarEmail(membro.Id, email);

            return (true, $"Inscrição realizada com Sucesso. Senha provissória enviada para o e-mail {TratarEmail(email)}");
        }

        public void AtualizarEmail(long id, string email)
        {
            _membroRepository.AtualizarEmail(id, email);
        }

        public (bool, string) RecuperarSenha(string cpf)
        {
            var membro = _membroRepository.GetByCPF(cpf, false);
            if (membro == null)
                return (false, "CPF nao localizado");

            if (string.IsNullOrEmpty(membro.Senha))
                return (false, "Membro não cadastrado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");

            var novaSenha = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6).ToUpper();
            var senhaCriptografada = Hash.GetHash(novaSenha, CryptoProviders.HashProvider.MD5);
            _membroRepository.AtualizarSenha(membro.Id, "", senhaCriptografada, true);

            var conteudo = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Infra", "Emails", "email_novasenha.txt"));
            conteudo = conteudo.Replace("[NovaSenha]", novaSenha);
            _emailAppService.EnviarEmail(membro.Email.ToLower(), "Nova Senha - Senha de acesso", conteudo);

            return (true, $"Senha provissória enviada para o e-mail {TratarEmail(membro.Email)}");
        }

        private string TratarEmail(string email)
        {
            var emailSplit = email.Split('@');

            var email1 = emailSplit[0].Substring(0, Math.Min(3, emailSplit[0].Length));

            Regex pattern = new("[a-zA-Z]");
            email1 += pattern.Replace(emailSplit[0].Substring(3), "*");

            var domain = emailSplit[1];
            var ponto = domain.IndexOf('.');

            var email2Length = ponto > 3 ? 3 : ponto;
            var email2 = domain.Substring(0, email2Length);
            email2 += pattern.Replace(domain.Substring(email2Length, ponto - email2Length), "*");
            email2 += domain.Substring(ponto);

            return $"{email1}@{email2}";
        }
    }
}

