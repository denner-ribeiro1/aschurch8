using System;
using ASChurchManager.Domain.Entities;

namespace ASChurchManager.Domain.Interfaces.Repository;

public interface ILoginAPIRepository
{
    Membro ConsultaPorCpf(string Cpf);

}
