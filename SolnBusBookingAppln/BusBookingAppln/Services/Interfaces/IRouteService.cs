using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Route;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IRouteService
    {
        public Task<int> GetRoute(string source, string destination);
        public Task<Models.DBModels.Route> GetRoute(int RouteId);
        public Task<AddRouteDTO> AddRoute(AddRouteDTO addRouteDTO);
    }
}
