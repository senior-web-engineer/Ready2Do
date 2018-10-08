using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PalestreGoGo.DataModel.Exceptions
{
    [Serializable]
    public class UserConfirmationException : ApplicationException
    {
        public UserConfirmationException() : base()
        {

        }

        public UserConfirmationException(string message) : base(message)
        {

        }

        public UserConfirmationException(string message, Exception innerException) :
            base(message, innerException)
        {

        }

        public UserConfirmationException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {

        }
    }
}
