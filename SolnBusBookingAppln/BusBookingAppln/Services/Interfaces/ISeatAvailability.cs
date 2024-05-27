using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ISeatAvailability
    {
        public Task<List<GetSeatsDTO>> GetAllAvailableSeatsInABusSchedule(int ScheduleId);
        public Task<bool> CheckSeatAvailability(Schedule schedule, int SeatID);
    }
}
