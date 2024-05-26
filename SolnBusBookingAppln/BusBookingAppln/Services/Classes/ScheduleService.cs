using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<int, Schedule> _ScheduleRepo;
        private readonly IRepository<int, BusRoute> _RouteRepo;

        public ScheduleService(IRepository<int, Schedule> ScheduleRepository, IRepository<int, BusRoute> RouteRepository)
        {
            _ScheduleRepo = ScheduleRepository;
            _RouteRepo = RouteRepository;
        }

        public async Task<AddScheduleDTO> AddSchedule(AddScheduleDTO addSchedulesDTO)
        {
            List<Schedule> Schedules = (List<Schedule>)await _ScheduleRepo.GetAll();
            if (!CheckIfBusAlreadyBooked(Schedules, addSchedulesDTO))
            {
                Schedule schedule = MapAddScheduleDTOToSchedule(addSchedulesDTO);
                await _ScheduleRepo.Add(schedule);
                return addSchedulesDTO;
            }
            throw new BusAlreadyBookedException();
        }

        private bool CheckIfBusAlreadyBooked(List<Schedule> schedules, AddScheduleDTO addScheduleDTO)
        {
            foreach(var  schedule in schedules)
            {
                if(schedule.BusNumber == addScheduleDTO.BusNumber)
                {
                    if((addScheduleDTO.DateTimeOfDeparture >= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfArrival) ||
                        (addScheduleDTO.DateTimeOfArrival >= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfArrival) ||
                        (addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfArrival >= schedule.DateTimeOfArrival))
                    {
                        return true;
                    }
                }
            }
            return false;
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
            int RouteId = await GetRoute(userInputDTOForSchedule.Source, userInputDTOForSchedule.Destination);
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
                ScheduleReturnDTO scheduleReturnDTO = new ScheduleReturnDTO();
                scheduleReturnDTO.Source = source;
                scheduleReturnDTO.Destination = dest;
                scheduleReturnDTO.DateTimeOfDeparture = schedule.DateTimeOfDeparture;
                scheduleReturnDTO.DateTimeOfArrival = schedule.DateTimeOfArrival;
                scheduleReturnDTO.BusNumber = schedule.BusNumber;
                scheduleReturnDTOs.Add(scheduleReturnDTO);
            }
            return scheduleReturnDTOs;
        }

        private async Task<int> GetRoute(string source, string destination)
        {
            var Routes = await _RouteRepo.GetAll();
            foreach (var route in Routes)
            {
                if (route.Source.ToLower() == source.ToLower() && route.Destination.ToLower() == destination.ToLower())
                {
                    return route.Id;
                }
            }
            throw new NoRoutesFoundForGivenSourceAndDest(source, destination);
        }

        public async Task<List<Seat>> GetAllAvailableSeatsInABusSchedule()
        {
            List<Schedule> schedules = (List<Schedule>)await _ScheduleRepo.GetAll();
            throw new NotImplementedException();
        }
    }
}
