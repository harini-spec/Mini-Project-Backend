using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class UserNotActiveException : Exception
    {
        public UserNotActiveException(string? message) : base(message)
        {
        }

    }
}