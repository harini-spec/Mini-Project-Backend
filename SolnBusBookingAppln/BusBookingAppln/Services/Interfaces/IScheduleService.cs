using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IScheduleService
    {
        public Task<AddScheduleDTO> AddSchedule(AddScheduleDTO Schedule);
        public Task<List<ScheduleReturnDTO>> GetAllSchedulesForAGivenDateAndRoute(UserInputDTOForSchedule userInputDTOForSchedule);
        public Task<List<Seat>> GetAllAvailableSeatsInABusSchedule();

    }
}
