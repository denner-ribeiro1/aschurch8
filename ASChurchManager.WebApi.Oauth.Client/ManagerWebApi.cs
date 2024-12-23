using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.WebApi.Oauth.Client
{
    public class AutenticacaoApiFITokenGeraToken
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string ClienteId { get; set; }

        public override bool Equals(object obj)
        {
            var o = (AutenticacaoApiFITokenGeraToken)obj;
            return (Usuario.Equals(o.Usuario, StringComparison.InvariantCultureIgnoreCase) &&
            Senha == o.Senha &&
            ClienteId.Equals(o.ClienteId));
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    internal class PersistenciaAccessToken
    {
        internal PersistenciaAccessToken()
        {
            Autenticacao = new AutenticacaoApiFITokenGeraToken();
            Parametros = new Dictionary<string, string>();
        }

        internal AutenticacaoApiFITokenGeraToken Autenticacao { get; set; }

        internal string UrlWebApi { get; set; }

        internal Dictionary<string, string> Parametros { get; set; }
    }

    public class ManagerWebApi
    {
        /// <summary>
        /// Constante que contém o código do sistema utilizado na geração dos códigos de erro
        /// </summary>
        public const int C_CODIGO_SISTEMA_ERRO = 99990000;

        /// <summary>
        /// Necessário gerar novo token para o usuário.
        /// </summary>
        public const int C_ERRO_AO_GERAR_ACESSTOKEN = C_CODIGO_SISTEMA_ERRO + 1;

        /// <summary>
        /// Necessário gerar novo token para o usuário.
        /// </summary>
        public const int C_ERRO_AO_SERIALIZAROBJETO = C_CODIGO_SISTEMA_ERRO + 2;

        private readonly AutenticacaoApiFITokenGeraToken Autenticacao;

        private readonly string UrlWebApi;

        public ManagerWebApi(AutenticacaoApiFITokenGeraToken Autenticacao, string UrlWebApi)
        {
            if (!UrlWebApi.EndsWith("/"))
                UrlWebApi += "/";
            this.Autenticacao = Autenticacao;
            this.UrlWebApi = UrlWebApi;
        }

        static readonly object locker = new object();

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="Json"></param>
        /// <returns></returns>
        public S Get<S>(string RotaMetodo)
        {
            return RequisitarWebApi<S>(RotaMetodo, TipoRequisicaoWebApi.Get, string.Empty, string.Empty, null);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="Json"></param>
        /// <returns></returns>
        public S Get<S>(string RotaMetodo, string ParametrosQueryString)
        {
            return Get<S>(RotaMetodo, ParametrosQueryString, null);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public S Get<S>(string RotaMetodo, Dictionary<string, string> Headers)
        {
            return Get<S>(RotaMetodo, string.Empty, Headers);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="ParametrosQueryString"></param>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public S Get<S>(string RotaMetodo, string ParametrosQueryString, Dictionary<string, string> Headers)
        {
            return RequisitarWebApi<S>(RotaMetodo, TipoRequisicaoWebApi.Get, string.Empty, ParametrosQueryString, Headers);
        }

        /// <summary>
        /// Efetua Post da requisição
        /// </summary>
        /// <typeparam name="S">Tipo do Retorno</typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="JsonPost"></param>
        /// <returns></returns>
        public S Post<S>(string RotaMetodo, string JsonPost)
        {
            return RequisitarWebApi<S>(RotaMetodo, TipoRequisicaoWebApi.Post, JsonPost, string.Empty, null);
        }

        /// <summary>
        /// Efetua Post da requisição
        /// </summary>
        /// <typeparam name="S">Tipo do Retorno</typeparam>
        /// <typeparam name="E">Tipo do Objeto do Post</typeparam>
        /// <param name="RotaMetodo">Rota do método api/Controller/Action, ex.: api/Clientes/Consultar</param>
        /// <param name="ObjetoPost">Objeto a ser postado</param>
        /// <returns></returns>
        public S Post<E, S>(string RotaMetodo, E ObjetoPost)
        {
            return Post<E, S>(RotaMetodo, ObjetoPost, string.Empty);
        }

        /// <summary>
        /// Efetua Post da requisição
        /// </summary>
        /// <typeparam name="S">Tipo do Retorno</typeparam>
        /// <typeparam name="E">Tipo do Objeto do Post</typeparam>
        /// <param name="RotaMetodo">Rota do método api/Controller/Action, ex.: api/Clientes/Consultar</param>
        /// <param name="ObjetoPost">Objeto a ser postado</param>
        /// <returns></returns>
        public S Post<E, S>(string RotaMetodo, E ObjetoPost, string ParametrosQueryString)
        {
            return Post<E, S>(RotaMetodo, ObjetoPost, ParametrosQueryString, null);
        }

        /// <summary>
        /// Efetua Post da requisição
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="ObjetoPost"></param>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public S Post<E, S>(string RotaMetodo, E ObjetoPost, Dictionary<string, string> Headers)
        {
            return Post<E, S>(RotaMetodo, ObjetoPost, string.Empty, Headers);
        }

        /// <summary>
        /// Efetua Post da requisição
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="ObjetoPost"></param>
        /// <param name="ParametrosQueryString"></param>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public S Post<E, S>(string RotaMetodo, E ObjetoPost, string ParametrosQueryString, Dictionary<string, string> Headers)
        {
            string JsonPost;
            try
            {
                if (ObjetoPost is string)
                    JsonPost = ObjetoPost.ToString();
                else
                    JsonPost = JsonConvert.SerializeObject(ObjetoPost);
            }
            catch
            {
                throw new Exception($"{C_ERRO_AO_SERIALIZAROBJETO} - {ObjetoPost.GetType()}");
            }
            return RequisitarWebApi<S>(RotaMetodo, TipoRequisicaoWebApi.Post, JsonPost, ParametrosQueryString, Headers);
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="JsonPost"></param>
        /// <param name="ParametrosQueryString"></param>
        /// <returns></returns>
        public T Post<T>(string RotaMetodo, string JsonPost, string ParametrosQueryString)
        {
            return Post<T>(RotaMetodo, JsonPost, ParametrosQueryString, null);
        }


        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="JsonPost"></param>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public T Post<T>(string RotaMetodo, string JsonPost, Dictionary<string, string> Headers)
        {
            return Post<T>(RotaMetodo, JsonPost, string.Empty, Headers);
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RotaMetodo"></param>
        /// <param name="JsonPost"></param>
        /// <param name="ParametrosQueryString"></param>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public T Post<T>(string RotaMetodo, string JsonPost, string ParametrosQueryString, Dictionary<string, string> Headers)
        {
            return RequisitarWebApi<T>(RotaMetodo, TipoRequisicaoWebApi.Post, JsonPost, ParametrosQueryString, Headers);
        }

        /// <summary>
        /// Requisitar WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlWebApi"></param>
        /// <param name="RotaMetodo"></param>
        /// <param name="clienteId"></param>
        /// <param name="sistema"></param>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <param name="codChave"></param>
        /// <param name="TipoRequisicao"></param>
        /// <param name="ParametrosQueryString"></param>
        /// <returns></returns>
        private T RequisitarWebApi<T>(string RotaMetodo, TipoRequisicaoWebApi TipoRequisicao, string JsonPost, string ParametrosQueryString, Dictionary<string, string> Headers)
        {
            Dictionary<string, string> retornoAccess = null;

            string urlToken = UrlWebApi + "oauth/Token";

            if (!RetornoAccessToken.Exists(x => x.UrlWebApi == UrlWebApi && x.Autenticacao.Equals(Autenticacao)))
            {
                //solicitar primeiro token
                retornoAccess = RequisitarAccessToken(urlToken, Autenticacao);
            }
            else if (RetornoAccessToken.First(x => x.UrlWebApi == UrlWebApi && x.Autenticacao.Equals(Autenticacao)).Parametros.ContainsKey(".expires")
                && DateTime.Parse(RetornoAccessToken.First(x => x.UrlWebApi == UrlWebApi && x.Autenticacao.Equals(Autenticacao)).Parametros[".expires"]) < DateTime.Now)
            {
                //se já existe token, porém expirou
                retornoAccess = AtualizarToken(UrlWebApi, urlToken, Autenticacao.ClienteId);

                //caso ocorra erro no refresh token
                if (retornoAccess.ContainsKey("error") && retornoAccess["error"].Equals("invalid_grant"))
                    retornoAccess = RequisitarAccessToken(urlToken, Autenticacao);
            }

            if (retornoAccess != null && retornoAccess.ContainsKey("error"))
            {
                lock (locker)
                    RetornoAccessToken.RemoveAll(x => x.UrlWebApi == UrlWebApi && x.Autenticacao.Equals(Autenticacao));

                throw new Exception($"{C_ERRO_AO_GERAR_ACESSTOKEN} - {JsonConvert.SerializeObject(retornoAccess)}");
            }
            else if (retornoAccess != null)
            {
                lock (locker)
                {
                    RetornoAccessToken.RemoveAll(x => x.UrlWebApi == UrlWebApi && x.Autenticacao.Equals(Autenticacao));

                    RetornoAccessToken.Add(new PersistenciaAccessToken() { UrlWebApi = UrlWebApi, Autenticacao = Autenticacao, Parametros = retornoAccess });
                }
            }

            return SolicitarWebApi<T>(UrlWebApi, RotaMetodo, JsonPost, ParametrosQueryString, TipoRequisicao, Headers);
        }

        /// <summary>
        /// [0] access_token
        /// [1] token_type
        /// [2] expires_in
        /// [3] userName
        /// [4] id": 
        /// [5] .issued": 
        /// [6] .expires":
        /// </summary>
        private static List<PersistenciaAccessToken> RetornoAccessToken { get; set; } = new List<PersistenciaAccessToken>();



        private Dictionary<string, string> RequisitarAccessToken(string urlToken, AutenticacaoApiFITokenGeraToken autenticacao)
        {
            using var client = new HttpClient();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var content = new FormUrlEncodedContent(new[]
            {
                        new KeyValuePair<string, string>("grant_type",  "astoken2"),
                        new KeyValuePair<string, string>("Client_ID", autenticacao.ClienteId),
                        new KeyValuePair<string, string>("login", autenticacao.Usuario)
                        //,new KeyValuePair<string, string>("senha", autenticacao.Senha) -- tipo astoken2 nao precisa enviar a senha.
                    });
            client.BaseAddress = new Uri(UrlWebApi);
            client.Timeout = new TimeSpan(0, 5, 0);
            Task<HttpResponseMessage> result = client.PostAsync(urlToken, content);
            result.Wait();

            Task<string> resultContent = result.Result.Content.ReadAsStringAsync();
            resultContent.Wait();
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(resultContent.Result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível requisitar AccessToken, retorno da API inválido: '{resultContent.Result}'.", ex);
            }
        }

        private Dictionary<string, string> AtualizarToken(string urlWebApi, string urlToken, string clientId)
        {

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("refresh_token", RetornoAccessToken.First(x => x.UrlWebApi == urlWebApi && x.Autenticacao.Equals(Autenticacao)).Parametros["access_token"]),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("Client_ID", clientId)
                });

            HttpResponseMessage result = client.PostAsync(urlToken, content).Result;

            Task<string> resultContent = result.Content.ReadAsStringAsync();
            resultContent.Wait();

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(resultContent.Result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível requisitar RefreshToken, retorno da API inválido: '{resultContent.Result}'.", ex);

            }
        }

        private T SolicitarWebApi<T>(string urlWebApi, string rotaMetodo, string conteudo, string parametros, TipoRequisicaoWebApi tipoRequisicao, Dictionary<string, string> headers)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + RetornoAccessToken.First(x => x.UrlWebApi == urlWebApi && x.Autenticacao.Equals(Autenticacao)).Parametros["access_token"]);

            if (headers != null)
                foreach (var item in headers)
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);

            Task<HttpResponseMessage> result = null;

            if (tipoRequisicao.Equals(TipoRequisicaoWebApi.Post))
            {
                var httpContent = new StringContent(conteudo, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                result = client.PostAsync(string.Concat(urlWebApi, rotaMetodo, parametros), httpContent);
                result.Wait();
            }
            else if (tipoRequisicao.Equals(TipoRequisicaoWebApi.Get))
            {
                result = client.GetAsync(string.Concat(urlWebApi, rotaMetodo, parametros));
                result.Wait();
            }

            Task<string> resultContent = result.Result.Content.ReadAsStringAsync();
            resultContent.Wait();

            try
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)resultContent.Result;
                else
                    return JsonConvert.DeserializeObject<T>(resultContent.Result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível requisitar '{rotaMetodo}', retorno da API inválido: '{resultContent.Result}'.", ex);
            }
        }
    }

}

