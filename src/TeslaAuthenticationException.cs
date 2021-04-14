using System;
using System.Runtime.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Raised when authentication fails.
    /// </summary>
    [Serializable]
    public class TeslaAuthenticationException : Exception
    {
        public int Status { get; set; }
        
        public TeslaAuthenticationException()
        {
        }

        public TeslaAuthenticationException(int code, string message) : base(message)
        {
        }

        public TeslaAuthenticationException(int code, string message, Exception inner) : base(message, inner)
        {
        }

        protected TeslaAuthenticationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}