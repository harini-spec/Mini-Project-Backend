
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Schedule;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<int, Schedule> _ScheduleRepo;
        private readonly IBusService _busService;
        private readonly IRouteService _RouteService;
        private readonly IDriverService _DriverService;

        public ScheduleService(IRepository<int, Schedule> ScheduleRepository, IBusService busService, IRouteService RouteService, IDriverService driverService)
        {
            _ScheduleRepo = ScheduleRepository;
            _busService = busService;
            _RouteService = RouteService;
            _DriverService = driverService;
        }

        public async Task<AddScheduleDTO> AddSchedule(AddScheduleDTO addSchedulesDTO)
        {
            await _busService.GetBusByBusNumber(addSchedulesDTO.BusNumber);
            List<Schedule> Schedules = new List<Schedule>();
            try
            {
                 Schedules = (List<Schedule>)await _ScheduleRepo.GetAll();
            }
            catch(Exception)
            {
                Schedule schedule = MapAddScheduleDTOToSchedule(addSchedulesDTO);
                await _ScheduleRepo.Add(schedule);
                return addSchedulesDTO;
            }
            if (!_busService.CheckIfBusAlreadyBooked(Schedules, addSchedulesDTO))
            {
                if(await _DriverService.CheckIfDriverAvailable(addSchedulesDTO, addSchedulesDTO.DriverId))
                {
                    Schedule schedule = MapAddScheduleDTOToSchedule(addSchedulesDTO);
                    await _ScheduleRepo.Add(schedule);
                    return addSchedulesDTO;
                }
                throw new DriverAlreadyBookedException();
            }
            throw new BusAlreadyBookedException();
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

        public async Task<List<ScheduleReturnDTO>> GetAllSchedulesForAGivenDateAndRoute(UserInputDTOForSchedule userInputDTOForSchedule)
        {
            int RouteId = await _RouteService.GetRoute(userInputDTOForSchedule.Source, userInputDTOForSchedule.Destination);
            var schedules = await _ScheduleRepo.GetAll();
            List<Schedule> result = new List<Schedule>();
            foreach (var schedule in schedules)
            {
                if (schedule.RouteId == RouteId && schedule.DateTimeOfDeparture.Date == userInputDTOForSchedule.DateTimeOfDeparture.Date)
                {
                    result.Add(schedule);
                }
            }
            if (result.Count == 0)
                throw new NoSchedulesFoundForGivenRouteAndDate();
            List<ScheduleReturnDTO> scheduleReturnDTOs = MapScheduleListToScheduleReturnDTOList(result, userInputDTOForSchedule.Source, userInputDTOForSchedule.Destination);
            return scheduleReturnDTOs;
        }

        private List<ScheduleReturnDTO> MapScheduleListToScheduleReturnDTOList(List<Schedule> result, string source, string dest)
        {
            List<ScheduleReturnDTO> scheduleReturnDTOs = new List<ScheduleReturnDTO>();
            foreach (var schedule in result)
            {
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleToScheduleReturnDTO(schedule, source, dest);
                scheduleReturnDTOs.Add(scheduleReturnDTO);
            }
            return scheduleReturnDTOs;
        }

        private ScheduleReturnDTO MapScheduleToScheduleReturnDTO(Schedule schedule, string source, string dest)
        {
            ScheduleReturnDTO scheduleReturnDTO = new ScheduleReturnDTO();
            scheduleReturnDTO.ScheduleId = schedule.Id;
            scheduleReturnDTO.Source = source;
            scheduleReturnDTO.Destination = dest;
            scheduleReturnDTO.DateTimeOfDeparture = schedule.DateTimeOfDeparture;
            scheduleReturnDTO.DateTimeOfArrival = schedule.DateTimeOfArrival;
            scheduleReturnDTO.BusNumber = schedule.BusNumber;
            return scheduleReturnDTO;
        }

        public async Task<Schedule> GetScheduleById(int ScheduleId)
        {
            return await _ScheduleRepo.GetById(ScheduleId);
        }

        private async Task<List<ScheduleReturnDTO>> MapScheduleListToScheduleReturnDTOList(List<Schedule> schedules)
        {
            List<ScheduleReturnDTO> scheduleReturnDTOs = new List<ScheduleReturnDTO>();
            foreach (var schedule in schedules)
            {
                BusRoute route = await _RouteService.GetRoute(schedule.RouteId);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleToScheduleReturnDTO(schedule, route.Source, route.Destination);
                scheduleReturnDTOs.Add(scheduleReturnDTO);
            }
            return scheduleReturnDTOs;
        }

        public async Task<List<ScheduleReturnDTO>> GetAllSchedules()
        {
            var schedules = await _ScheduleRepo.GetAll();
            List<ScheduleReturnDTO> scheduleReturnDTOs = await MapScheduleListToScheduleReturnDTOList(schedules.ToList());
            return scheduleReturnDTOs;
        }
    }
}
