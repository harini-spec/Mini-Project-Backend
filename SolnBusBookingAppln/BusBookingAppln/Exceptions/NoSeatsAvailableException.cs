using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class NoSeatsAvailableException : Exception
    {
        string msg;
        public NoSeatsAvailableException()
        {
            msg = "No seats available";
        }
        public NoSeatsAvailableException(List<int> seatsNotAvailable)
        {
            msg = "Following seats are not available: " + string.Join(",", seatsNotAvailable.ToArray());
        }
        public override string Message => msg;
    }
}