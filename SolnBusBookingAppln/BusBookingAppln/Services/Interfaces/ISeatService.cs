using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ISeatService
    {
        public Task<Seat> GetSeatById(int seatId);
    }
}
