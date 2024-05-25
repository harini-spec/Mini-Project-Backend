using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IBusService
    {
        public Task<AddBusDTO> AddBus(AddBusDTO bus);
    }
}
