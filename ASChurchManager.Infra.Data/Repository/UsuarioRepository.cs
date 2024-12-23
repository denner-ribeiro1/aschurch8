using ASBaseLib.Data.DataMapping;
using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ASChurchManager.Infra.Data.Repository
{
    public class UsuarioRepository : RepositoryDAO<Usuario>, IUsuarioRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public UsuarioRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string ConsUsuarioPorUserName = "dbo.ConsultarUsuarioPorUserName";
        private const string ConsultarUsuario = "dbo.ConsultarUsuario";
        private const string ListarUsuarios = "dbo.ListarUsuarios";
        private const string SalvarUsuario = "dbo.SalvarUsuario";
        private const string DeletarUsuario = "dbo.DeletarUsuario";
        private const string VerificarLogin = "dbo.ValidarLogin";
        private const string AlterarSenhaUsuario = "dbo.AlterarSenhaUsuario";
        private const string ExisteUsuario = "dbo.ExisteUsuario";
        private const string AltSkinUsuario = "AlterarSkinUsuario";
        private const string ListarUsuPaginada = "ListarUsuariosPaginada";

        #endregion

        #region Publicos
        public Usuario GetUsuarioByUsername(string userName)
        {
            Usuario usuario = new Usuario();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@UserName", userName);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                                                                            ConsUsuarioPorUserName, sqlParameter))
                {
                    while (dr.Read())
                    {
                        usuario = DataMapper.ExecuteMapping<Usuario>(dr);

                        if (dr["CongregacaoId"] != DBNull.Value)
                        {
                            usuario.Congregacao = new Congregacao()
                            {
                                Id = Convert.ToInt64(dr["CongregacaoId"]),
                                Nome = dr["CongregacaoNome"].ToString(),
                                Sede = dr["CongregacaoSede"].TryConvertTo<bool>()
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return usuario;
        }

        public override long Add(Usuario usuario, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", usuario.Id),
                    new SqlParameter("@Nome", usuario.Nome),
                    new SqlParameter("@Username", usuario.Username),
                    new SqlParameter("@Senha", usuario.Senha),
                    new SqlParameter("@Email", usuario.Email),
                    new SqlParameter("@AlterarSenhaProxLogin", usuario.AlterarSenhaProxLogin),
                    new SqlParameter("@CongregacaoId", usuario.Congregacao.Id),
                    new SqlParameter("@PerfilId", usuario.Perfil.Id),
                    new SqlParameter("@PermiteAprovarMembro", usuario.PermiteAprovarMembro),
                    new SqlParameter("@PermiteImprimirCarteirinha", usuario.PermiteImprimirCarteirinha),
                    new SqlParameter("@PermiteExcluirSituacaoMembro", usuario.PermiteExcluirSituacaoMembro),
                    new SqlParameter("@PermiteCadBatismoAposDataMaxima", usuario.PermiteCadBatismoAposDataMaxima),
                    new SqlParameter("@PermiteExcluirMembros", usuario.PermiteExcluirMembros),
                    new SqlParameter("@PermiteExcluirCargoMembro", usuario.PermiteExcluirCargoMembro),
                    new SqlParameter("@PermiteExcluirObservacaoMembro", usuario.PermiteExcluirObservacaoMembro)
                };
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                            SalvarUsuario, lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Usuario usuario, long usuarioID = 0)
        {
            try
            {
                return Add(usuario);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override int Delete(Usuario usuario, long usuarioID = 0)
        {
            return Delete(usuario.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                        DeletarUsuario, sqlParameter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Usuario GetById(long id, long usuarioID = 0)
        {
            Usuario usuario = new Usuario();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                                                                            ConsultarUsuario, sqlParameter))
                {
                    while (dr.Read())
                    {
                        usuario = GetUsuarioFromDataReader(usuario, dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return usuario;
        }

        public override IEnumerable<Usuario> GetAll(long usuarioID = 0)
        {
            List<Usuario> lstUsuario = new List<Usuario>();

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarUsuarios))
                {
                    while (dr.Read())
                    {
                        var usuario = DataMapper.ExecuteMapping<Usuario>(dr);
                        lstUsuario.Add(GetUsuarioFromDataReader(usuario, dr));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstUsuario;
        }

        public bool ValidarLogin(ref Usuario usuario)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Username", usuario.Username),
                    new SqlParameter("@Senha", usuario.Senha)
                };

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                                                                            VerificarLogin, lstParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        usuario = GetUsuarioFromDataReader(usuario, dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return usuario != null && !string.IsNullOrEmpty(usuario.Nome);
        }

        private static Usuario GetUsuarioFromDataReader(Usuario usuario, SqlDataReader dr)
        {
            usuario = DataMapper.ExecuteMapping<Usuario>(dr);

            if (dr["CongregacaoId"] != DBNull.Value)
            {
                usuario.Congregacao = new Congregacao()
                {
                    Id = Convert.ToInt64(dr["CongregacaoId"]),
                    Nome = dr["CongregacaoNome"].ToString(),
                    Sede = dr["CongregacaoSede"].TryConvertTo<bool>()
                };
            }

            if (dr["PerfilId"] != DBNull.Value)
            {
                usuario.Perfil = new Perfil()
                {
                    Id = Convert.ToInt64(dr["PerfilId"]),
                    Nome = dr["PerfilNome"].ToString(),
                    TipoPerfil = (TipoPerfil)dr["PerfilTipo"]
                };
            }
            return usuario;
        }

        public long AlterarSenha(Usuario usuario, long usuarioAlteracaoId = 0)
        {
            try
            {
                var lstParams = new List<SqlParameter>
                {
                    new SqlParameter("@Id", usuario.Id),
                    new SqlParameter("@Senha", usuario.Senha),
                    new SqlParameter("@UsuarioAlteracao", usuarioAlteracaoId)
                };
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                        AlterarSenhaUsuario, lstParams.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool VerificaUsuarioDuplicado(string username)
        {
            bool existeUsuario = false;

            try
            {
                SqlParameter param = new SqlParameter("@Username", username);

                existeUsuario = Convert.ToBoolean(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure,
                                                                                    ExisteUsuario, param));
            }
            catch (System.Exception)
            {
                throw;
            }

            return existeUsuario;
        }

        public bool AlterarSkinUsuario(string skin, long id)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Skin", skin),
                    new SqlParameter("@UsuarioID", id),
                };
                MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, 
                    AltSkinUsuario, lstParans.ToArray());
                return true;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<Usuario> ListarUsuariosPaginada(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID)
        {
            rowCount = 0;
            List<Usuario> lstUsuario = new List<Usuario>();
            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@PAGESIZE", pageSize),
                    new SqlParameter("@ROWSTART", rowStart),
                    new SqlParameter("@SORTING", sorting),
                    new SqlParameter("@USUARIOID", usuarioID),
                    new SqlParameter("@CAMPO", _campo),
                    new SqlParameter("@VALOR", _valor)
                };

            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ListarUsuPaginada, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var usuario = new Usuario()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                Username = ds.Tables[0].Rows[i]["Username"].ToString(),
                                Email = ds.Tables[0].Rows[i]["Email"].ToString(),
                                CongregacaoNome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString()
                            };
                            usuario.Congregacao.Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString();

                            lstUsuario.Add(usuario);
                        }
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstUsuario;
        }
        #endregion
    }
}