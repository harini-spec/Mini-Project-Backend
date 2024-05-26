using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class BusAlreadyBookedException : Exception
    {
        string msg;
        public BusAlreadyBookedException()
        {
            msg = "Bus is already scheduled for a different ride";
        }
        public override string Message => msg;
    }
}