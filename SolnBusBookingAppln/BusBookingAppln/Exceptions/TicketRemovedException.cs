using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class TicketRemovedException : Exception
    {
        public TicketRemovedException(string? message) : base(message)
        {
        }
    }
}