using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    [Serializable]
    internal class IncorrectOperationException : Exception
    {
        public IncorrectOperationException()
        {
        }

        public IncorrectOperationException(string? message) : base(message)
        {
        }

        public IncorrectOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected IncorrectOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}