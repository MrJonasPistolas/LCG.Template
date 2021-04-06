using System;
using System.Runtime.Serialization;

namespace LCG.Template.Common.Exceptions.Application
{
    public class RoleCreationException : Exception
    {
        public RoleCreationException()
        {
        }

        public RoleCreationException(string message) : base(message)
        {
        }

        public RoleCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RoleCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
