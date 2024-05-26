using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class NoSchedulesFoundForGivenRouteAndDate : Exception
    {
        string msg;
        public NoSchedulesFoundForGivenRouteAndDate()
        {
            msg = "No schedules found in the given route and on the given date";
        }
        public override string Message => msg;
    }
}