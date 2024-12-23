//using ASBaseLib.Core.Common.Validation;
using System;

namespace ASChurchManager.Domain.Types
{
    public struct CPF
    {
        private string _numeroCpf;

        public string NumeroCpf
        {
            get { return _numeroCpf; }
            set
            {
                if (!Validar(value))
                {
                    throw new InvalidOperationException("Número de CPF inválido");
                }
                _numeroCpf = value;
            }
        }

        //public CPF(string numeroCpf)
        //    : this()
        //{
        //    if (!Validar(numeroCpf))
        //    {
        //        throw new InvalidOperationException("Número de CPF inválido");
        //    }
        //}

        public bool Validar(string numeroCpf)
        {
            _numeroCpf = _numeroCpf ?? numeroCpf;
            return false; //DocumentValidation.ValidateCpf(this._numeroCpf);
        }
    }
}