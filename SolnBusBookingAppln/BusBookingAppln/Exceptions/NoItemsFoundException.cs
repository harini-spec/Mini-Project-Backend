using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class NoItemsFoundException : Exception
    {
        public NoItemsFoundException()
        {
        }

        public NoItemsFoundException(string? message) : base(message)
        {
        }

        public NoItemsFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoItemsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}