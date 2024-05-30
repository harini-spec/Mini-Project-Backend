using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class ValidationErrorExcpetion : Exception
    {
        public ValidationErrorExcpetion(string? message) : base(message)
        {
        }
    }
}