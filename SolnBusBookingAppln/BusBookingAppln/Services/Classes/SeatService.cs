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
        private readonly ILogger<SeatService> _logger;

        public SeatService(IRepository<int, Seat> seatRepo, ILogger<SeatService> logger)
        {
            _SeatRepo = seatRepo;
            _logger = logger;
        }

        public async Task<Seat> GetSeatById(int seatId)
        {
            try
            {
                Seat seat = await _SeatRepo.GetById(seatId);
                return seat;
            }
            catch(EntityNotFoundException enf)
            {
                _logger.LogError(enf.Message);
                throw;
            }
        }
    }
}
