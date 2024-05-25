using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class DataDoesNotMatchException : Exception
    {
        string msg;
        public DataDoesNotMatchException()
        {
            msg = "Total seats in bus and the number of seats entered do not match";
        }
        public override string Message => msg; 
    }
}