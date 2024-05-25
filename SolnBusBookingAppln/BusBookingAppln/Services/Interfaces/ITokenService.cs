using BusBookingAppln.Models.DBModels;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken<T>(T user); // Admin, Customer and Driver
    }
}
