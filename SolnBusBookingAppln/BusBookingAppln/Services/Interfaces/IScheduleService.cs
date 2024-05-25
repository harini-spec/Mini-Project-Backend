using BusBookingAppln.Models.DTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IScheduleService
    {
        public Task<AddScheduleDTO> AddSchedule(AddScheduleDTO Schedule);
    }
}
