using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Schedule;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IScheduleService
    {
        public Task<AddScheduleDTO> AddSchedule(AddScheduleDTO Schedule);
        public Task<Schedule> GetScheduleById(int ScheduleId);
        public Task<List<ScheduleReturnDTO>> GetAllSchedulesForAGivenDateAndRoute(UserInputDTOForSchedule userInputDTOForSchedule);
        public Task<List<ScheduleReturnDTO>> GetAllSchedules();
        public Task<List<ScheduleReturnDTO>> GetAllSchedulesOfDriver(int DriverId);
    }
}
