using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Exceptions;

namespace BusBookingAppln.Controllers
{
    public class SeatController : ControllerBase
    {
        private readonly ILogger<SeatService> _Logger;
        private readonly ISeatService _seatService;

        public SeatController(ILogger<SeatService> logger, ISeatService seatService)
        {
            _seatService = seatService;
            _Logger = logger;
        }

        [HttpGet("GetSeatsOfBus")]
        [Authorize(Roles = "Admin, Customer")]
        [ProducesResponseType(typeof(List<GetSeatsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetSeatsDTO>>> GetSeatsOfBus(string BusNumber)
        {
            try
            {
                List<GetSeatsDTO> result = await _seatService.GetSeatsOfBus(BusNumber);
                return Ok(result);
            }
            catch (EntityNotFoundException enf)
            {
                _Logger.LogError(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (NoItemsFoundException nif)
            {
                _Logger.LogError(nif.Message);
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }


        [HttpGet("GetSeatsBySeatId")]
        [Authorize(Roles = "Admin, Customer")]
        [ProducesResponseType(typeof(GetSeatsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetSeatsDTO>> GetSeatBySeatId(int SeatId)
        {
            try
            {
                var result = await _seatService.GetSeatById(SeatId);
                return Ok(result);
            }
            catch (EntityNotFoundException enf)
            {
                _Logger.LogError(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }
    }
}
