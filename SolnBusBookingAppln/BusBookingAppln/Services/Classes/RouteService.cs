using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Route;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class RouteService : IRouteService
    {
        private readonly IRepository<int, Models.DBModels.Route> _RouteRepository;

        public RouteService(IRepository<int, Models.DBModels.Route> RouteRepository)
        {
            _RouteRepository = RouteRepository;
        }

        public async Task<int> GetRoute(string source, string destination)
        {
            var Routes = await _RouteRepository.GetAll();
            var Route = Routes.ToList().FirstOrDefault(x => x.Source.ToLower() == source.ToLower() && x.Destination.ToLower() == destination.ToLower());
            if(Route != null)
                return Route.Id;
            throw new NoRoutesFoundForGivenSourceAndDest(source, destination);
        }

        public async Task<Models.DBModels.Route> GetRoute(int RouteId)
        {
            return await _RouteRepository.GetById(RouteId);
        }

        public async Task<AddRouteDTO> AddRoute(AddRouteDTO addRouteDTO)
        {
            Models.DBModels.Route route = MapAddRouteDTOToRoute(addRouteDTO);
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

        private Models.DBModels.Route MapAddRouteDTOToRoute(AddRouteDTO addRouteDTO)
        {
            Models.DBModels.Route route = new Models.DBModels.Route();
            route.Source = addRouteDTO.Source;
            route.Destination = addRouteDTO.Destination;
            return route;
        }
    }
}
