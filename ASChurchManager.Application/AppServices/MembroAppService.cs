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
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.Formats.Png;
using System.Net.Http;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;
using SystemFonts = SixLabors.Fonts.SystemFonts;
using Font = SixLabors.Fonts.Font;
using PointF = SixLabors.ImageSharp.PointF;
using SixLabors.ImageSharp.Drawing.Processing;
using Color = SixLabors.ImageSharp.Color;
using QRCoder;

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

        public void AtualizarSenha(long Id, string SenhaAtual, string NovaSenha, bool atualizarSenha, bool atualizarDataInscricao = false)
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

        public (bool, string) InscricaoApp(string cpf, string nomeMae, DateTime dataNascimento)
        {
            var membro = _membroRepository.GetByCPF(cpf, false);

            if (membro != null && membro.Id == 0)
                return (false, "CPF não localizado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro");

            if (membro.Status != Status.Ativo)
                return (false, "Membro não localizado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");

            if (string.IsNullOrEmpty(membro.Email))
                return (false, "E-mail não localizado! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");

            if (membro.DataInscricaoApp != DateTime.MinValue)
                return (false, "Membro ja cadastrado no App! Favor utilizar a opção 'Esqueceu Senha' na tela incial.");

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
                return (false, "Data de Nascimento não corresponde ao Cadastro! Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");



            var (senha, senhaCriptografada) = GerarSenha();
            _membroRepository.AtualizarSenha(membro.Id, "", senhaCriptografada, true, true);

            var conteudo = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "emails", "email_inscricao.txt"));
            conteudo = conteudo.Replace("[NovaSenha]", senha);
            conteudo = conteudo.Replace("[Membro]", membro.Nome);

            var mail = new Email()
            {
                Assunto = "Inscrição - Senha de acesso",
                Corpo = conteudo,
                Endereco = membro.Email.ToLower(),
                MembroId = (int)membro.Id
            };

            mail.Id = (int)_emailAppService.SalvarEmail(mail);
            _emailAppService.RequisitarEnvioEmail(mail.Id);


            return (true, $"Inscrição realizada com Sucesso. Senha provissória enviada para o e-mail {TratarEmail(membro.Email)}");
        }
        private (string, string) GerarSenha()
        {
            Random rmd = new();
            string senha = rmd.Next(100000, 999999).ToString();
            var senhaCriptografada = Hash.GetHash(senha, CryptoProviders.HashProvider.MD5);
            return (senha, senhaCriptografada); //senha,senhaCriptografada

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
                return (false, "Membro não realizou o cadastro! Favor utilizar a opção 'Inscreva-se' na tela incial. Caso tenha a necessidade de mais alguma atualizaçao de dados, favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro.");


            var (senha, senhaCriptografada) = GerarSenha();
            _membroRepository.AtualizarSenha(membro.Id, "", senhaCriptografada, true);

            var conteudo = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "emails", "email_novasenha.txt"));
            conteudo = conteudo.Replace("[NovaSenha]", senha);
            conteudo = conteudo.Replace("[Membro]", membro.Nome);


            var mail = new Email()
            {
                Assunto = "Nova Senha - Senha de acesso",
                Corpo = conteudo,
                Endereco = membro.Email.ToLower(),
                MembroId = (int)membro.Id
            };

            mail.Id = (int)_emailAppService.SalvarEmail(mail);
            _emailAppService.RequisitarEnvioEmail(mail.Id);

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
        public (bool, byte[], string) CarterinhaFrente(Carteirinha membro, Image image)
        {
            try
            {
                // Baixar a imagem da URL
                if (!string.IsNullOrWhiteSpace(membro.FotoUrl))
                {
                    byte[] imageBytes;
                    using (HttpClient client = new())
                    {
                        imageBytes = client.GetByteArrayAsync(membro.FotoUrl).Result;
                    }

                    // Carregar a imagem da URL
                    using (Image overlayImage = Image.Load(imageBytes))
                    {
                        var height = (float)overlayImage.Height;
                        var width = (float)overlayImage.Width;
                        var perc = 280.0 / (float)height;
                        var heightAlt = height * perc;
                        var widthAlt = width * perc;

                        overlayImage.Mutate(ctx => ctx.Resize(Convert.ToInt32(widthAlt), Convert.ToInt32(heightAlt)));

                        // Definir posição da imagem sobreposta
                        Point overlayPosition = new Point(750, 230); // Ajuste X e Y conforme necessário

                        // Sobrepor a imagem
                        image.Mutate(ctx => ctx.DrawImage(overlayImage, overlayPosition, 1f)); // 1f = Opacidade total
                    }
                }

                // Configurar fontes
                Font fontLarge = SystemFonts.CreateFont("DejaVu Sans", 25); // Fonte grande
                Font fontSmall = SystemFonts.CreateFont("DejaVu Sans", 20); // Fonte pequena

                // Configurar posições
                PointF posicaoNome, posicaoRM, posicaoMembroDesde, posicaoCargo, posicaoValidade;

                if (membro.TipoCarteirinha == Domain.Types.TipoCarteirinha.Membro)
                {
                    // Posições para "membro_frente.png"
                    posicaoNome = new PointF(75, 375);
                    posicaoRM = new PointF(75, 445);
                    posicaoMembroDesde = new PointF(290, 445);
                    posicaoCargo = PointF.Empty;
                    posicaoValidade = PointF.Empty;
                }
                else
                {
                    // Posições para "obreiro_frente.png"
                    posicaoCargo = new PointF(75, 330);
                    posicaoValidade = new PointF(445, 335);
                    posicaoNome = new PointF(75, 390);
                    posicaoRM = new PointF(75, 460);
                    posicaoMembroDesde = new PointF(285, 465);
                }

                // Aplicar textos na imagem
                image.Mutate(ctx =>
                {
                    if (membro.TipoCarteirinha != Domain.Types.TipoCarteirinha.Membro)
                    {
                        ctx.DrawText(membro.TipoCarteirinha.GetDisplayAttributeValue() ?? string.Empty, fontLarge, Color.Black, posicaoCargo);
                        ctx.DrawText(membro.DataValidadeCarteirinha ?? string.Empty, fontSmall, Color.Black, posicaoValidade);
                    }

                    ctx.DrawText(membro.Nome, fontLarge, Color.Black, posicaoNome);
                    ctx.DrawText(membro.Id.ToString(), fontLarge, Color.Black, posicaoRM);
                    if (!string.IsNullOrWhiteSpace(membro.DataRecepcao))
                        ctx.DrawText(membro.DataRecepcao, fontSmall, Color.Black, posicaoMembroDesde);
                });

                // Rotacionar a imagem 
                image.Mutate(ctx => ctx.Rotate(RotateMode.Rotate90));

                // Salvar imagem em memória
                var memoryStream = new MemoryStream();
                image.Save(memoryStream, new PngEncoder());

                return (true, memoryStream.ToArray(), "Carteirinha gerada com sucesso!");
            }
            catch (Exception ex)
            {
                return (false, null, $"Erro ao gerar a carteirinha: {ex.Message}");
            }
        }



        public (bool, byte[], string) CarterinhaVerso(Carteirinha membro, Image image)
        {
            try
            {
                Font font = SystemFonts.CreateFont("DejaVu Sans", 20); // Nome da fonte e tamanho
                Font font2 = SystemFonts.CreateFont("DejaVu Sans", 15); // Nome da fonte e tamanho

                PointF posicaoPai, posicaoMae, posicaoCidade, posicaoUf, posicaoDataNascimento, posicaoEstadoCivil, posicaoCpf, posicaoDataBatismo, posicaoDataConsagracao, posicaoConfragesp, posicaoCgadb;

                if (membro.TipoCarteirinha == Domain.Types.TipoCarteirinha.Membro)
                {
                    // Posições para "membro_frente.png"
                    posicaoPai = new PointF(70, 90);
                    posicaoMae = new PointF(70, 150);
                    posicaoCidade = new PointF(70, 220);
                    posicaoUf = new PointF(860, 220);
                    posicaoDataNascimento = new PointF(70, 290);
                    posicaoEstadoCivil = new PointF(320, 290);
                    posicaoCpf = new PointF(320, 360);
                    posicaoDataBatismo = new PointF(70, 360);
                    posicaoDataConsagracao = PointF.Empty;
                    posicaoConfragesp = PointF.Empty;
                    posicaoCgadb = PointF.Empty;
                }
                else
                {
                    // Posições para "obreiro_verso.png"
                    posicaoPai = new PointF(70, 90);
                    posicaoMae = new PointF(70, 155);
                    posicaoCidade = new PointF(70, 220);
                    posicaoUf = new PointF(880, 220);
                    posicaoDataNascimento = new PointF(70, 280);
                    posicaoEstadoCivil = new PointF(320, 280);
                    posicaoCpf = new PointF(610, 280);
                    posicaoDataBatismo = new PointF(70, 345);
                    posicaoDataConsagracao = new PointF(320, 345);
                    posicaoConfragesp = new PointF(70, 410);
                    posicaoCgadb = new PointF(320, 410);
                }

                // Aplicar o texto na imagem, verificando valores nulos
                image.Mutate(ctx =>
                {
                    ctx.DrawText(membro.NomePai ?? string.Empty, font, Color.Black, posicaoPai);
                    ctx.DrawText(membro.NomeMae ?? string.Empty, font, Color.Black, posicaoMae);
                    ctx.DrawText(membro.Cidade ?? string.Empty, font, Color.Black, posicaoCidade);
                    ctx.DrawText(membro.Estado ?? string.Empty, font, Color.Black, posicaoUf);
                    ctx.DrawText(membro.DataNascimento ?? string.Empty, font, Color.Black, posicaoDataNascimento);
                    ctx.DrawText(membro.EstadoCivil ?? string.Empty, font, Color.Black, posicaoEstadoCivil);
                    ctx.DrawText(membro.Cpf ?? string.Empty, font, Color.Black, posicaoCpf);
                    ctx.DrawText(membro.DataBatismoAguas ?? string.Empty, font, Color.Black, posicaoDataBatismo);
                    ctx.DrawText(membro.DataConsagracao ?? string.Empty, font, Color.Black, posicaoDataConsagracao);
                    ctx.DrawText(membro.Cgadb ?? string.Empty, font, Color.Black, posicaoCgadb);
                    ctx.DrawText(membro.Confradesp ?? string.Empty, font, Color.Black, posicaoConfragesp);
                });

                // Salvar a imagem com as informações preenchidas
                //  image.Mutate(ctx => ctx.Rotate(RotateMode.Rotate90));

                var memoryStream = new MemoryStream();
                image.Save(memoryStream, new PngEncoder()); // Salvar como PNG no stream

                return (true, memoryStream.ToArray(), "Carterinha gerada com sucesso!");
            }
            catch (Exception ex)
            {
                return (false, null, $"Erro ao gerar a carteirinha: {ex.Message}");
            }
        }
        public (bool, byte[], string) GerarQrCode(Carteirinha membro)
        {
            string chave64 = GerarIdBase64((int)membro.Id);
            var imageAsByte = GenerateByteArray(chave64, _configuration);
            return (true, imageAsByte, "QrCode gerado com sucesso!");
        }
        private static byte[] GenerateImage(string valor, IConfiguration _configuration)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(valor, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(6);
            return qrCodeImage;
        }
        private static byte[] GenerateByteArray(string url, IConfiguration _configuration) => GenerateImage(url, _configuration);

        private string GerarIdBase64(int id)
        {
            string chave = $"RM{id.ToString().PadLeft(9, '0')}";
            byte[] chaveAsByte = System.Text.Encoding.ASCII.GetBytes(chave);
            return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(chaveAsByte);
        }

        public bool AtualizarMembroAtualizado(long id, bool atualizado)
        {

            return _membroRepository.AtualizarMembroAtualizado(id, atualizado);
        }


    }

}

