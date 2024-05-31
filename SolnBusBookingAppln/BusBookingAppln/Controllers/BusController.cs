using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly ILogger<BusController> _logger;

        public BusController(IBusService busService, ILogger<BusController> logger)
        {
            _busService = busService;
            _logger = logger;
        }

        [HttpPost("AddBusAndSeats")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AddBusDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AddBusDTO>> AddBus(AddBusDTO bus)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddBusDTO result = await _busService.AddBus(bus);
                    return Ok(result);
                }
                catch(DataDoesNotMatchException ddnm)
                {
                    _logger.LogError(ddnm.Message);
                    return UnprocessableEntity(new ErrorModel(422, ddnm.Message));
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
