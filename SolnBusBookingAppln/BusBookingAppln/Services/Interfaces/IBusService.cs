using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.Schedule;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IBusService
    {
        public Task<AddBusDTO> AddBus(AddBusDTO bus);
        public Task<Bus> GetBusByBusNumber(string BusNumber);
        public bool CheckIfBusAlreadyBooked(List<Schedule> schedules, AddScheduleDTO addScheduleDTO);
    }
}
