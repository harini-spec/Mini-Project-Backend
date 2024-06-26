using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Schedule;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IScheduleService scheduleService, ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        [HttpPost("AddSchedule")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AddScheduleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AddScheduleDTO>> AddSchedule(AddScheduleDTO schedule)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AddScheduleDTO result = await _scheduleService.AddSchedule(schedule);
                    return Ok(result);
                }
                catch (DriverAlreadyBookedException dabe)
                {
                    _logger.LogError(dabe.Message);
                    return Conflict(new ErrorModel(409, dabe.Message));
                }
                catch (BusAlreadyBookedException babe)
                {
                    _logger.LogError(babe.Message);
                    return Conflict(new ErrorModel(409, babe.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [Authorize(Roles = "Admin, Customer")]
        [HttpPost("BusesScheduledOnGivenDateAndRoute")]
        [ProducesResponseType(typeof(List<ScheduleReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduleReturnDTO>>> GetAllSchedulesForAGivenDateAndRoute(UserInputDTOForSchedule userInput)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<ScheduleReturnDTO> result = await _scheduleService.GetAllSchedulesForAGivenDateAndRoute(userInput);
                    return Ok(result);
                }
                catch (NoRoutesFoundForGivenSourceAndDest nrf)
                {
                    _logger.LogError(nrf.Message);
                    return NotFound(new ErrorModel(404, nrf.Message));
                }
                catch (NoSchedulesFoundForGivenRouteAndDate nsf)
                {
                    _logger.LogError(nsf.Message);
                    return NotFound(new ErrorModel(404, nsf.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [Authorize(Roles = "Admin, Customer")]
        [HttpGet("GetAllSchedules")]
        [ProducesResponseType(typeof(List<ScheduleReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduleReturnDTO>>> GetAllSchedules()
        {
                try
                {
                    List<ScheduleReturnDTO> result = await _scheduleService.GetAllSchedules();
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
