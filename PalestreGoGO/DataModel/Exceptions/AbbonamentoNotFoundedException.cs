using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PalestreGoGo.DataModel.Exceptions
{
    public class AbbonamentoNotFoundedException : ApplicationException
    {
        public AbbonamentoNotFoundedException()
        {
        }

        public AbbonamentoNotFoundedException(string message)
        : base(message)
        {
        }

        public AbbonamentoNotFoundedException(string message, Exception inner)
        : base(message, inner)
        {
        }

        public AbbonamentoNotFoundedException(SerializationInfo info, StreamingContext context) :
          base(info, context)
        {

        }
    }
}
