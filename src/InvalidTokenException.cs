using System;
using System.Runtime.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Thrown when an API call fails with a 401 - Unauthorized response due to the token
    /// being invalid. This often means the token needs to be refreshed.
    /// </summary>
    [Serializable]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string message) : base(message)
        {
        }

        public InvalidTokenException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidTokenException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}