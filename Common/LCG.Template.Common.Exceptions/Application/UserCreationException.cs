using System;
using System.Runtime.Serialization;

namespace LCG.Template.Common.Exceptions.Application
{
    public class UserCreationException : Exception
    {
        public UserCreationException()
        {
        }

        public UserCreationException(string message) : base(message)
        {
        }

        public UserCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
