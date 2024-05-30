
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
        private readonly IRepository<int, Driver> _driverWithSchedulesRepo;
        private readonly IBusService _busService;
        private readonly IRouteService _RouteService;
        private readonly IDriverService _DriverService;


        public ScheduleService(IRepository<int, Driver> driverWithSchedulesRepo, IRepository<int, Schedule> ScheduleRepository, IBusService busService, IRouteService RouteService, IDriverService driverService)
        {
            _ScheduleRepo = ScheduleRepository;
            _busService = busService;
            _RouteService = RouteService;
            _DriverService = driverService;
            _driverWithSchedulesRepo = driverWithSchedulesRepo;
        }


        #region AddSchedule

        // Add schedule
        public async Task<AddScheduleDTO> AddSchedule(AddScheduleDTO addSchedulesDTO)
        {
            // Check if given Bus is present
            await _busService.GetBusByBusNumber(addSchedulesDTO.BusNumber);


            List<Schedule> Schedules = new List<Schedule>();
            // If no schedules present, add schedule without checking for driver and bus availability
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


            // Check if bus is free on given date and time
            if (!_busService.CheckIfBusAlreadyBooked(Schedules, addSchedulesDTO))
            {
                // Check if driver is free on given date and time
                if (await _DriverService.CheckIfDriverAvailable(addSchedulesDTO, addSchedulesDTO.DriverId))
                {
                    Schedule schedule = MapAddScheduleDTOToSchedule(addSchedulesDTO);
                    await _ScheduleRepo.Add(schedule);
                    return addSchedulesDTO;
                }
                throw new DriverAlreadyBookedException();
            }
            throw new BusAlreadyBookedException();
        }

        #endregion


        #region GetAllSchedulesForAGivenDateAndRoute

        // Get all schedules for given date and route
        public async Task<List<ScheduleReturnDTO>> GetAllSchedulesForAGivenDateAndRoute(UserInputDTOForSchedule userInputDTOForSchedule)
        {
            // Get route by source and dest
            int RouteId = await _RouteService.GetRoute(userInputDTOForSchedule.Source, userInputDTOForSchedule.Destination);

            // Filter all schedules in the given route, date of departure and arrival
            var schedules = await _ScheduleRepo.GetAll();
            var result = schedules.ToList().Where(x => x.RouteId == RouteId && x.DateTimeOfDeparture.Date == userInputDTOForSchedule.DateTimeOfDeparture.Date);
            
            if (result.ToList().Count == 0)
                throw new NoSchedulesFoundForGivenRouteAndDate();
            List<ScheduleReturnDTO> scheduleReturnDTOs = MapScheduleListToScheduleReturnDTOList(result.ToList(), userInputDTOForSchedule.Source, userInputDTOForSchedule.Destination);
            return scheduleReturnDTOs;
        }

        #endregion


        #region GetScheduleById

        public async Task<Schedule> GetScheduleById(int ScheduleId)
        {
            return await _ScheduleRepo.GetById(ScheduleId);
        }

        #endregion


        #region GetAllSchedules

        // Get all future schedules
        public async Task<List<ScheduleReturnDTO>> GetAllSchedules()
        {
            var schedules = await _ScheduleRepo.GetAll();

            // Get Schedules scheduled after this point of time
            schedules = schedules.Where(schedule => schedule.DateTimeOfDeparture > DateTime.Now).ToList();
            List<ScheduleReturnDTO> scheduleReturnDTOs = await MapScheduleListToScheduleReturnDTOList(schedules.ToList());
            if(scheduleReturnDTOs.Count == 0)
            {
                throw new NoItemsFoundException("No Schedules are found");
            }
            return scheduleReturnDTOs;
        }

        #endregion


        #region GetAllSchedulesOfDriver

        // Get all the schedules of driver
        public async Task<List<ScheduleReturnDTO>> GetAllSchedulesOfDriver(int DriverId)
        {
            Driver driver = await _driverWithSchedulesRepo.GetById(DriverId);
            List<Schedule> schedules = driver.SchedulesForDriver.ToList();

            // Get Schedules scheduled after this point of time
            schedules = schedules.Where(schedule => schedule.DateTimeOfDeparture > DateTime.Now).ToList();
            List<ScheduleReturnDTO> scheduleReturnDTOs = await MapScheduleListToScheduleReturnDTOList(schedules);
            if (schedules.Count == 0)
            {
                throw new NoItemsFoundException($"No Schedules are found for Driver with Id {DriverId}.");
            }
            return scheduleReturnDTOs;
        }

        #endregion


        #region Mappers

        // Map Schedule List to ScheduleReturnDTO List
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


        // Map AddScheduleDTO to Schedule
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


        // Map Schedule List to ScheduleReturnDTO List
        private async Task<List<ScheduleReturnDTO>> MapScheduleListToScheduleReturnDTOList(List<Schedule> schedules)
        {
            List<ScheduleReturnDTO> scheduleReturnDTOs = new List<ScheduleReturnDTO>();
            foreach (var schedule in schedules)
            {
                Models.DBModels.Route route = await _RouteService.GetRoute(schedule.RouteId);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleToScheduleReturnDTO(schedule, route.Source, route.Destination);
                scheduleReturnDTOs.Add(scheduleReturnDTO);
            }
            return scheduleReturnDTOs;
        }


        // Map Schedule to ScheduleReturnDTO
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

        #endregion

    }
}
