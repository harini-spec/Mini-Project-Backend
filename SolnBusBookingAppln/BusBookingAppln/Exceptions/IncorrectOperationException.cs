using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class IncorrectOperationException : Exception
    {

        public IncorrectOperationException(string? message) : base(message)
        {
        }
    }
}