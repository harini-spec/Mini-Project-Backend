using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class RouteService : IRouteService
    {
        private readonly IRepository<int, BusRoute> _RouteRepository;

        public RouteService(IRepository<int, BusRoute> RouteRepository)
        {
            _RouteRepository = RouteRepository;
        }

        public async Task<AddRouteDTO> AddRoute(AddRouteDTO addRouteDTO)
        {
            BusRoute route = MapAddRouteDTOToRoute(addRouteDTO);
            await _RouteRepository.Add(route);
            route.RouteStops = MapAddRouteDetailToRouteDetail(route.Id, addRouteDTO);
            await _RouteRepository.Update(route, route.Id);
            return addRouteDTO;
        }

        private List<RouteDetail> MapAddRouteDetailToRouteDetail(int RouteId, AddRouteDTO addRouteDTO)
        {
            List<RouteDetail> routeDetails = new List<RouteDetail>();
            foreach(var RouteStop in addRouteDTO.RouteStops)
            {
                RouteDetail routeDetail = new RouteDetail();
                routeDetail.RouteId = RouteId;
                routeDetail.StopNumber = RouteStop.StopNumber;
                routeDetail.From_Location = RouteStop.From_Location;
                routeDetail.To_Location = RouteStop.To_Location;
                routeDetails.Add(routeDetail);
            }
            return routeDetails;
        }

        private BusRoute MapAddRouteDTOToRoute(AddRouteDTO addRouteDTO)
        {
            BusRoute route = new BusRoute();
            route.Source = addRouteDTO.Source;
            route.Destination = addRouteDTO.Destination;
            return route;
        }
    }
}
