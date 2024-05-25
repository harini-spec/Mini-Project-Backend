using BusBookingAppln.Models.DTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IRouteService
    {
        public Task<AddRouteDTO> AddRoute(AddRouteDTO addRouteDTO);
    }
}
