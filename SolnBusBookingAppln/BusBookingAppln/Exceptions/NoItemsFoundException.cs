using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class NoItemsFoundException : Exception
    {
        public NoItemsFoundException(string? message) : base(message)
        {
        }
    }
}