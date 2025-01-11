using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;

namespace ASChurchManager.Infra.Data.Repository;

public class EmailRepository : RepositoryDAO<Email>, IEmailRepository
{
    #region Variaveis Private
    private readonly IConfiguration _configuration;
    private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
    #endregion
    #region Construtor
    public EmailRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    #endregion

    #region Constantes
    private const string AtualizarStatusEmail = "AtualizarStatusEmail";
    private const string ListarEmailsPendentes = "ListarEmailsPendentes";
    private const string SalvarEmail = "SalvarEmail";

    #endregion


    public override long Add(Email entity, long usuarioID = 0)
    {
        var lstParans = new List<SqlParameter>
                    {

                        new("@MembroID", entity.MembroId),
                        new("@EmailAddress",entity.EmailAddress ),
                        new("@Subject", entity.Assunto),
                        new("@Body", entity.Corpo)
                    };

        return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarEmail, lstParans.ToArray()));

    }

    public override int Delete(Email entity, long usuarioID = 0)
    {
        throw new NotImplementedException();
    }

    public override int Delete(long id, long usuarioID = 0)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Email> GetAll(long usuarioID)
    {
        var lEmails = new List<Email>();
        using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarEmailsPendentes))
        {
            while (dr.Read())
            {
                var mail = DataMapper.ExecuteMapping<Email>(dr);
                lEmails.Add(mail);
            }
        }
        return lEmails;
    }



    public override Email GetById(long id, long usuarioID)
    {
        throw new NotImplementedException();
    }

    public override long Update(Email entity, long usuarioID = 0)
    {
        var lstParans = new List<SqlParameter>
                    {

                        new("@Id", entity.Id)

                    };

        return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, AtualizarStatusEmail, lstParans.ToArray()));
    }
}
