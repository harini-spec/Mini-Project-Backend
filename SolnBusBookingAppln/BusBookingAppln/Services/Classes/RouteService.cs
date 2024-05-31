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
        private readonly ILogger<RouteService> _logger;

        public RouteService(IRepository<int, Models.DBModels.Route> RouteRepository, ILogger<RouteService> logger)
        {
            _RouteRepository = RouteRepository;
            _logger = logger;
        }


        #region GetRoute With source and dest

        // Get Route ID with Source and Destination
        public async Task<int> GetRoute(string source, string destination)
        {
            var Routes = await _RouteRepository.GetAll();
            var Route = Routes.ToList().FirstOrDefault(x => x.Source.ToLower() == source.ToLower() && x.Destination.ToLower() == destination.ToLower());
            if(Route != null)
                return Route.Id;
            _logger.LogError($"No Route found for source = {source} and dest = {destination}");
            throw new NoRoutesFoundForGivenSourceAndDest(source, destination);
        }

        #endregion


        #region GetRoute With Route ID

        // Get Route ID with ID
        public async Task<Models.DBModels.Route> GetRoute(int RouteId)
        {
            return await _RouteRepository.GetById(RouteId);
        }

        #endregion


        #region Get All routes 

        public async Task<List<RouteReturnDTO>> GetAllRoutes()
        {
            try
            {
                var routes = await _RouteRepository.GetAll();
                List<RouteReturnDTO> routeDTOs = MapRouteToRouteReturnDTO(routes.ToList());
                return routeDTOs;
            }
            catch (NoItemsFoundException)
            {
                throw;
            }
        } 

        #endregion


        #region AddRoute

        // Add Route with stops
        public async Task<RouteDTO> AddRoute(RouteDTO addRouteDTO)
        {
            Models.DBModels.Route route = MapRouteDTOToRoute(addRouteDTO);
            route.RouteStops = MapRouteDetailToRouteDetail(route.Id, addRouteDTO);
            await _RouteRepository.Add(route);
            return addRouteDTO;
        }

        #endregion


        #region Mappers

        // Map Route to RouteDTO
        private List<RouteReturnDTO> MapRouteToRouteReturnDTO(List<Models.DBModels.Route> routes)
        {
            List<RouteReturnDTO> routeDTOs = new List<RouteReturnDTO>();    
            foreach(var route in routes)
            {
                RouteReturnDTO routeDTO = new RouteReturnDTO();
                routeDTO.RouteId = route.Id;
                routeDTO.Source = route.Source;
                routeDTO.Destination = route.Destination;
                routeDTO.RouteStops = new List<RouteDetailsDTO>();
                routeDTO.RouteStops = MapRouteDetailToRouteDetailDTO(route.RouteStops.ToList());
                routeDTOs.Add(routeDTO);
            }
            return routeDTOs;
        }

        // Map RouteDetail to RouteDetail DTO
        private List<RouteDetailsDTO> MapRouteDetailToRouteDetailDTO(List<RouteDetail> routeStops)
        {
            List<RouteDetailsDTO> routeDetailsDTOs = new List<RouteDetailsDTO>();
            foreach(var stop in routeStops)
            {
                RouteDetailsDTO routeDetailsDTO = new RouteDetailsDTO();
                routeDetailsDTO.StopNumber = stop.StopNumber;
                routeDetailsDTO.To_Location = stop.To_Location;
                routeDetailsDTO.From_Location = stop.From_Location;
                routeDetailsDTOs.Add(routeDetailsDTO);
            }
            return routeDetailsDTOs;
        }

        // Map RouteDTO to RouteDetail
        private List<RouteDetail> MapRouteDetailToRouteDetail(int RouteId, RouteDTO addRouteDTO)
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


        // Map RouteDTO to Route
        private Models.DBModels.Route MapRouteDTOToRoute(RouteDTO addRouteDTO)
        {
            Models.DBModels.Route route = new Models.DBModels.Route();
            route.Source = addRouteDTO.Source;
            route.Destination = addRouteDTO.Destination;
            return route;
        }

        #endregion

    }
}
