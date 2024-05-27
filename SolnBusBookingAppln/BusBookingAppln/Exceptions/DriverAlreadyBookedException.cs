using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class DriverAlreadyBookedException : Exception
    {
        string msg;
        public DriverAlreadyBookedException()
        {
            msg = "Driver already booked for a different schedule";
        }
        public override string Message => msg; 
    }
}