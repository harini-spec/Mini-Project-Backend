using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Route;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IRouteService
    {
        #region Summary
        /// <summary>
        /// Get Route with given source and destination
        /// </summary>
        /// <param name="source">Source area string</param>
        /// <param name="destination">Destination area string</param>
        /// <returns>RouteId with given source and destination</returns>
        #endregion
        public Task<int> GetRoute(string source, string destination);

        #region Summary
        /// <summary>
        /// Get Route object with given ID
        /// </summary>
        /// <param name="RouteId">ID of the route to be retrieved</param>
        /// <returns>Route object with ID</returns>
        #endregion
        public Task<Models.DBModels.Route> GetRoute(int RouteId);

        #region Summary
        /// <summary>
        /// Gets all routes
        /// </summary>
        /// <returns>Route DTO list containing all route information</returns>
        #endregion 
        public Task<List<RouteReturnDTO>> GetAllRoutes();

        #region Summary
        /// <summary>
        /// Add route with intermediate stops
        /// </summary>
        /// <param name="addRouteDTO">Add route DTO with stop DTO list</param>
        /// <returns>Added route DTO with stop DTO list</returns>
        #endregion 
        public Task<RouteDTO> AddRoute(RouteDTO addRouteDTO);
    }
}
