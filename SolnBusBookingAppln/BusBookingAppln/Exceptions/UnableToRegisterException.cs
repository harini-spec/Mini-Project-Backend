using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class UnableToRegisterException : Exception
    {
        public UnableToRegisterException(string? message) : base(message)
        {
        }

    }
}