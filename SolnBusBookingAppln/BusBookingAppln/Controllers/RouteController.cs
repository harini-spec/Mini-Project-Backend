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
        [ProducesResponseType(typeof(RouteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteDTO>> AddRoute(RouteDTO route)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RouteDTO result = await _routeService.AddRoute(route);
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

        [HttpGet("GetAllRoutes")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<RouteReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RouteReturnDTO>>> GetAllRoutes()
        {
                try
                {
                    var result = await _routeService.GetAllRoutes();
                    return Ok(result);
                }
                catch (NoItemsFoundException nif)
                {
                    _logger.LogError(nif.Message);
                    return NotFound(new ErrorModel(404, nif.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
        }
    }
}
