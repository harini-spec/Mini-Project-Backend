using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
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
                catch (InvalidOperationException ioe)
                {
                    return Conflict(new ErrorModel(409, ioe.Message));
                }
                catch (Exception ex)
                {
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
        public async Task<ActionResult<List<ScheduleReturnDTO>>> GetBusesScheduledOnGivenDateAndRoute(UserInputDTOForSchedule userInput)
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
                    return NotFound(new ErrorModel(404, nrf.Message));
                }
                catch (NoSchedulesFoundForGivenRouteAndDate nsf)
                {
                    return NotFound(new ErrorModel(404, nsf.Message));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }
    }
}
