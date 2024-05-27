using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class SeatService : ISeatService
    {
        private readonly IRepository<int, Seat> _SeatRepo;

        public SeatService(IRepository<int, Seat> seatRepo)
        {
            _SeatRepo = seatRepo;
        }

        public async Task<Seat> GetSeatById(int seatId)
        {
            return await _SeatRepo.GetById(seatId);
        }
    }
}
