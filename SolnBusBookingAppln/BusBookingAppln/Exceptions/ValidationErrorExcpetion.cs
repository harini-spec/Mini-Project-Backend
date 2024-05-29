using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    [Serializable]
    internal class ValidationErrorExcpetion : Exception
    {
        public ValidationErrorExcpetion()
        {
        }

        public ValidationErrorExcpetion(string? message) : base(message)
        {
        }

        public ValidationErrorExcpetion(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ValidationErrorExcpetion(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}