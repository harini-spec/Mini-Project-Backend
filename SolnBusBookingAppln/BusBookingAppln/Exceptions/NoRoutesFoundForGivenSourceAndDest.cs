using System.Runtime.Serialization;

namespace BusBookingAppln.Exceptions
{
    public class NoRoutesFoundForGivenSourceAndDest : Exception
    {
        string msg;
        public NoRoutesFoundForGivenSourceAndDest(string source, string dest)
        {
            msg = $"No routes found from {source} to {dest}";
        }
        public override string Message => msg;
    }
}