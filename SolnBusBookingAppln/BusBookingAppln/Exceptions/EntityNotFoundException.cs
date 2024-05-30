using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string? message) : base(message)
        {
        }
    }
}