using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class UnauthorizedUserException : Exception
    {

        public UnauthorizedUserException(string? message) : base(message)
        {
        }
    }
}