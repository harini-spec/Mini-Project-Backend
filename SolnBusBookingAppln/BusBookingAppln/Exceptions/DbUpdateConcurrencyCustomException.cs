using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class DbUpdateConcurrencyCustomException : Exception
    {
        string msg;
        public DbUpdateConcurrencyCustomException()
        {
            msg = "Database exception: Unexpected number of rows are affected during save - Concurrency violation";
        }
        public override string Message => msg;
    }
}