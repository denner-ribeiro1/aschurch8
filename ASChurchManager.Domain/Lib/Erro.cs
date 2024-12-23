using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ASChurchManager.Domain.Lib
{
    public class Erro : Exception
    {
        public Erro()
        {
        }

        public Erro(string message) : base(message)
        {
        }

        public Erro(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
