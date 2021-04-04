using System;
using System.Runtime.Serialization;

namespace Julmar.TeslaApi
{
    public sealed class SleepingException : Exception
    {
        public SleepingException()
        {
        }

        public SleepingException(string message) : base(message)
        {
        }

        public SleepingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SleepingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
