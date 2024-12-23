using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using Microsoft.Extensions.Configuration;

namespace ASChurchManager.Infra.Data.Repository;

public class LoginAPIRepository : ILoginAPIRepository
{
    private readonly IConfiguration _configuration;
    private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
    private const string ConsultarMembroPorCpf = "ConsultaMembroPorCPF";
    private const string ListarSituacoes = "dbo.ListarSituacaoMembro";
    private const string ListarCargos = "dbo.ListarCargosMembro";

    public LoginAPIRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Membro ConsultaPorCpf(string Cpf)
    {
        var membro = new Membro();

        var lstParameters = new List<SqlParameter>
            {
                new("@Cpf", Cpf),

            };


        using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                   ConsultarMembroPorCpf, lstParameters.ToArray()))
        {
            while (dr.Read())
            {
                membro = DataMapper.ExecuteMapping<Membro>(dr);


                membro.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                membro.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();

                membro.Conjuge = new Membro();
                if (int.TryParse(dr["IdConjuge"].ToString(), out int IdConj) && IdConj > 0)
                {
                    membro.Conjuge.Id = Convert.ToInt64(dr["IdConjuge"].ToString());
                    membro.Conjuge.Nome = dr["NomeConjuge"].TryConvertTo<string>();
                }

                membro.Pai = new Membro();
                if (int.TryParse(dr["IdPai"].ToString(), out int IdPai) && IdPai > 0)
                {
                    membro.Pai.Id = Convert.ToInt64(dr["IdPai"].ToString());
                    membro.Pai.Nome = dr["NomePai"].TryConvertTo<string>();
                }

                membro.Mae = new Membro();
                if (int.TryParse(dr["IdMae"].ToString(), out int IdMae) && IdMae > 0)
                {
                    membro.Mae.Id = Convert.ToInt64(dr["IdMae"].ToString());
                    membro.Mae.Nome = dr["NomeMae"].TryConvertTo<string>();
                }

                membro.DataNascimento = membro.DataNascimento == DateTimeOffset.MinValue ? null : membro.DataNascimento;
                membro.DataBatismoAguas = membro.DataBatismoAguas == DateTimeOffset.MinValue ? null : membro.DataBatismoAguas;
                membro.DataRecepcao = membro.DataRecepcao == DateTimeOffset.MinValue ? null : membro.DataRecepcao;
            }
        }

        var situacoes = ListarSituacoesMembro(membro.Id);
        var cargos = ListarCargosMembro(membro.Id);

        membro.Situacoes = situacoes.ToList();
        membro.Cargos = cargos.ToList();

        return membro;
    }
    public IEnumerable<SituacaoMembro> ListarSituacoesMembro(long membroId)
    {

        var lstSituacoes = new List<SituacaoMembro>();

        try
        {
            var param = new SqlParameter("@MembroId", membroId);

            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarSituacoes, param))
            {
                while (dr.Read())
                {
                    var situacao = DataMapper.ExecuteMapping<SituacaoMembro>(dr);
                    lstSituacoes.Add(situacao);
                }
            }
        }
        catch (System.Exception)
        {
            throw;
        }

        return lstSituacoes;
    }
    public IEnumerable<CargoMembro> ListarCargosMembro(long membroId)
    {
        var cargos = new List<CargoMembro>();
        try
        {
            var param = new SqlParameter("@MembroId", membroId);

            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarCargos, param))
            {
                while (dr.Read())
                {
                    var situacao = DataMapper.ExecuteMapping<CargoMembro>(dr);
                    cargos.Add(situacao);
                }
            }
        }
        catch (System.Exception)
        {
            throw;
        }

        return cargos;
    }



}
