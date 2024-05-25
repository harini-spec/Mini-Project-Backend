using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class InvalidOperationCustomException : Exception
    {
        string msg;
        public InvalidOperationCustomException()
        {
            msg = "Invalid operation : Key already present in DB";
        }
        public override string Message => msg;
    }
}