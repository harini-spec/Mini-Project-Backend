using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Route;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpPost("AddRouteAndStops")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AddRouteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AddRouteDTO>> AddRoute(AddRouteDTO route)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddRouteDTO result = await _routeService.AddRoute(route);
                    return Ok(result);
                }
                catch (InvalidOperationException ioe)
                {
                    _logger.LogError(ioe.Message);
                    return Conflict(new ErrorModel(409, ioe.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }
    }
}
