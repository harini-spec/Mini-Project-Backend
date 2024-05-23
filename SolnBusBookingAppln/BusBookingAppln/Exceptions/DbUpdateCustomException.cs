using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class DbUpdateCustomException : Exception
    {
        string msg;
        public DbUpdateCustomException()
        {
            msg = "Database exception: DbUpdateException";
        }
        public override string Message => msg;
    }
}