using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<int, Schedule> _ScheduleRepository;

        public ScheduleService(IRepository<int, Schedule> ScheduleRepository)
        {
            _ScheduleRepository = ScheduleRepository;
        }

        public async Task<AddScheduleDTO> AddSchedule(AddScheduleDTO addSchedulesDTO)
        {
            Schedule schedule = MapAddScheduleDTOToSchedule(addSchedulesDTO);
            await _ScheduleRepository.Add(schedule);
            return addSchedulesDTO;
        }

        private Schedule MapAddScheduleDTOToSchedule(AddScheduleDTO addSchedulesDTO)
        {
            Schedule schedule = new Schedule();
            schedule.DriverId = addSchedulesDTO.DriverId;
            schedule.DateTimeOfDeparture = addSchedulesDTO.DateTimeOfDeparture;
            schedule.DateTimeOfArrival = addSchedulesDTO.DateTimeOfArrival;
            schedule.BusNumber = addSchedulesDTO.BusNumber;
            schedule.RouteId = addSchedulesDTO.RouteId;
            return schedule;
        }
    }
}
