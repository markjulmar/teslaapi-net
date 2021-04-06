using System;
using System.Runtime.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Thrown when the vehicle is asleep and unable to accept commands. You can call
    /// <see cref="Vehicle.WakeAsync"/> to wake the vehicle up.
    /// </summary>
    [Serializable]
    public class SleepingException : Exception
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
