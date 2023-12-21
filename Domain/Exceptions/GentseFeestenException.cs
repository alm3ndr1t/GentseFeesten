using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class GentseFeestenException : Exception
    {
        public GentseFeestenException()
        {
        }

        public GentseFeestenException(string? message) : base(message)
        {
        }

        public GentseFeestenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected GentseFeestenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
